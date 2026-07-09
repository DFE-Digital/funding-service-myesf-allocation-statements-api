using AllocationStatementsApi.Services.Helpers;
using AllocationStatementsApi.Services.Interfaces;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.ApiClient.Services;

namespace AllocationStatementsApi.Services.DependencyInjection
{
    /// <summary>
    /// Extensions class for <see cref="IServiceCollection"/> for registering the feature's services.
    /// </summary>
    public static class FeatureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for the current feature to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add the feature's services to.
        /// </param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFeatureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAllocationRepository>(s => CosmosHelper.GetCosmosDbRepository(services, configuration));
            services.AddSingleton<IAllocationSearchService>(s => SearchHelper.GetAzureAllocationSearchService(configuration));

            services.AddTransient(typeof(IAuthenticationService<>), typeof(AuthenticationService<>));

            return services;
        }
    }
}