using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using RestApiDemo.WebApi.Exceptions;
using BadHttpRequestException = Microsoft.AspNetCore.Http.BadHttpRequestException;

namespace RestApiDemo.WebApi.Middleware
{
    /// <summary>
    ///     Converts each exception that occurs in the application to an appropriate HTTP response.
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private static readonly RouteData _emptyRouteData = new RouteData();
        private static readonly ActionDescriptor _emptyAction = new ActionDescriptor();
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger,
            IHostingEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) when (ex is IWebApiException wae)
            {
                await ProcessWebApiException(httpContext, ex, wae.Result);
            }
            // TODO: Replace this exception with Microsoft.AspNetCore.Http.BadHttpRequestException when switching to asp.net core 3.1
            catch (Exception ex) when (ex is BadHttpRequestException)
            {
                // Pass the system exception through, otherwise we are breaking this part of ASP.Net:
                // https://github.com/dotnet/aspnetcore/blob/b463e049b6aed02f94edcac8855b8b5c87d0989b/src/Servers/Kestrel/Core/src/Internal/Http2/Http2Connection.cs#L1200
                throw;
            }
            catch (Exception ex)
            {
                await ProcessException(httpContext, ex);
            }
        }

        private static async Task ExecuteResult(HttpContext httpContext, IActionResult result)
        {
            RouteData routeData = httpContext.GetRouteData() ?? _emptyRouteData;
            var actionContext = new ActionContext(httpContext, routeData, _emptyAction);
            await result.ExecuteResultAsync(actionContext);
        }

        private async Task ProcessException(HttpContext httpContext, Exception ex)
        {
            _logger.LogError($"An unhandled exception has occurred: {ex}");
            string detail = _env.IsProduction() ? null : ex.ToString();
            var details = new ProblemDetails
            {
                Type = ErrorTypes.InternalServerError,
                Title = "A server error has occurred.",
                Detail = detail
            };

            var result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };

            await ExecuteResult(httpContext, result);
        }

        private async Task ProcessWebApiException(
            HttpContext httpContext,
            Exception ex,
            IActionResult result)
        {
            _logger.LogWarning($"{ex.GetType().Name}: {ex.Message}");
            await ExecuteResult(httpContext, result);
        }
    }
}