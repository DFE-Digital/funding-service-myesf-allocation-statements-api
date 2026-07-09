using AllocationStatementsApi;
using AllocationStatementsApi.Helpers;
using AllocationStatementsApi.Services.DependencyInjection;
using Microsoft.OpenApi.Models;
using Pds.Core.ApiAuthentication;
using Pds.Core.Logging;
using Pds.Core.Telemetry.ApplicationInsights;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// TODO: Replace AutoMapper with free alternative.
// TODO: After we replace AutoMapper, remove WarningsNotAsErrors NU1901,NU1902,NU1903,NU1904 from .csproj files
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddApiControllers();

builder.Services.AddFeatureServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

var currentApiVersion = "v1.0.0";
var requireElevatedRightsPolicyName = "RequireElevatedRights";

builder.Services.AddPdsApplicationInsightsTelemetry(options => ProgramHelper.BuildAppInsightsConfiguration(builder.Configuration, options, assemblyName));
builder.Services.AddLoggerAdapter();
builder.Services.AddAzureADAuthentication(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.DisableAuthentication(assemblyName);
}

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(currentApiVersion, new OpenApiInfo { Title = assemblyName, Version = currentApiVersion });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{assemblyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
    if (!builder.Environment.IsDevelopment())
    {
        ProgramHelper.AddOauth2BearerTokenAuthDefinition(c);
    }
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(requireElevatedRightsPolicyName, policy => policy.RequireRole("AllocationsApiRole"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"/swagger/{currentApiVersion}/swagger.json", assemblyName);
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();