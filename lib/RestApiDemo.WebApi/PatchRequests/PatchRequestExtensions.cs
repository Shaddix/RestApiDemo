using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestApiDemo.WebApi.Serialization;

namespace RestApiDemo.WebApi.PatchRequests
{
    public static class PatchRequestExtensions
    {
        public static void AddPatchRequest(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddNewtonsoftJson(
                    setupAction => SetupJson(setupAction.SerializerSettings));

            services
                .AddControllers()
                .AddNewtonsoftJson(
                    setupAction =>
                    {
                        var settings = setupAction.SerializerSettings;
                        settings.Converters = new List<JsonConverter>
                        {
                            new StringEnumConverter(),
                            new DefaultDateTimeConverter(),
                        };
                        // otherwise date is automatically converted, which will prevent us from handling TimezoneIndependentDate
                        settings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    });
        }


        private static JsonSerializerSettings SetupJson(JsonSerializerSettings settings)
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
                new StringEnumConverter(),
                new DefaultDateTimeConverter(),
            };
            // otherwise date is automatically converted, which will prevent us from handling TimezoneIndependentDate
            settings.DateParseHandling = DateParseHandling.DateTimeOffset;
            // settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            return settings;
        }
    }
}