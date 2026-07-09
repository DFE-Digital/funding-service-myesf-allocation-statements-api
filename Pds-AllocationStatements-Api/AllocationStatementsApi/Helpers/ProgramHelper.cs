using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Pds.Core.Telemetry.ApplicationInsights;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace AllocationStatementsApi.Helpers
{
    /// <summary>
    /// Helper class for Program.
    /// </summary>
    public static class ProgramHelper
    {
        /// <summary>
        /// Adds OAuth Authentication.
        /// </summary>
        /// <param name="c">SwaggerGenOptions.</param>
        public static void AddOauth2BearerTokenAuthDefinition(SwaggerGenOptions c)
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            });
        }

        /// <summary>
        /// Builds Application Insights Configuration.
        /// </summary>
        /// <param name="configuration">the Configuration to bind to.</param>
        /// <param name="options">PdsApplicationInsightsConfiguration options.</param>
        /// <param name="assemblyName">The assembly name.</param>
        public static void BuildAppInsightsConfiguration(IConfiguration configuration, PdsApplicationInsightsConfiguration options, string assemblyName)
        {
            configuration.Bind("PdsApplicationInsights", options);
            options.Component = assemblyName;
        }
    }
}
