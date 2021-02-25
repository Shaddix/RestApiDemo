using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using RestApiDemo.WebApi.PatchRequests;

namespace RestApiDemo.WebApi.NSwag
{
    public static class NSwagExtensions
    {
        public static void AddSwagger(this IServiceCollection services,
            SwaggerOptions swaggerOptions)
        {
            services.AddOpenApiDocument(
                options =>
                {
                    options.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token",
                        new OpenApiSecurityScheme
                        {
                            Type = OpenApiSecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            Description = "Copy 'Bearer ' + valid JWT token into field",
                            In = OpenApiSecurityApiKeyLocation.Header
                        }));


                    options.PostProcess = document =>
                    {
                        document.Info = new OpenApiInfo
                        {
                            Version = swaggerOptions.Version,
                            Title = swaggerOptions.Title,
                            Description = swaggerOptions.Description,
                            Contact = new OpenApiContact
                            {
                                Email = swaggerOptions.Contact.Email
                            },
                            License = new OpenApiLicense
                            {
                                Name = swaggerOptions.License.Name
                            },
                        };
                    };
                    //
                    // options.AddSecurity("Bearer", new OpenApiSecurityScheme()
                    // {
                    //     Type = OpenApiSecuritySchemeType.OAuth2,
                    //     Description = "Authentication",
                    //     Flow = OpenApiOAuth2Flow.Password,
                    //     Flows = new OpenApiOAuthFlows()
                    //     {
                    //         Password = new OpenApiOAuthFlow()
                    //         {
                    //             TokenUrl = "/connect/token",
                    //             RefreshUrl = "/connect/token",
                    //             AuthorizationUrl = "/connect/token",
                    //             Scopes = new Dictionary<string, string>()
                    //             {
                    //                 {"profile", "profile"},
                    //                 {"offline_access", "offline_access"},
                    //                 {"WebAPI", "WebAPI"},
                    //             }
                    //         }
                    //     }
                    // });
                    options.OperationProcessors.Add(
                        new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
                    options.SchemaProcessors.Add(new RequireValueTypesSchemaProcessor());
                    options.FlattenInheritanceHierarchy = true;
                    options.GenerateEnumMappingDescription = true;
                });
        }

        public static void UseSwagger(this IApplicationBuilder app, SwaggerOptions swaggerOptions)
        {
            if (!swaggerOptions.Enabled)
            {
                return;
            }

            app.UseOpenApi(
                options => { options.Path = "/swagger/v1/swagger.json"; });
            app.UseSwaggerUi3(
                options =>
                {
                    options.Path = "/swagger";
                    options.DocumentPath = "/swagger/v1/swagger.json";
                });
        }

        public static JsonSerializerSettings SetupJson(JsonSerializerSettings settings)
        {
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.ContractResolver = new PatchRequestContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false,
                    OverrideSpecifiedNames = false
                }
            };
            settings.Converters = new List<JsonConverter>
            {
                // new StringEnumConverter()
            };
            return settings;
        }
    }
}