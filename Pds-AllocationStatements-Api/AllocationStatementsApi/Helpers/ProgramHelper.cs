using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Pds.Core.Telemetry.ApplicationInsights;
using Swashbuckle.AspNetCore.SwaggerGen;

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

            const string schemeId = JwtBearerDefaults.AuthenticationScheme;

            c.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(schemeId, document)] = []
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
