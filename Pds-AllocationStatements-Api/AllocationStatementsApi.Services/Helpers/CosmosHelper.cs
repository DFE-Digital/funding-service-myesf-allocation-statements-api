using AllocationStatementsApi.Services.Configuration;
using AllocationStatementsApi.Services.Implementations.IFileMetadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.Core.Logging;

namespace AllocationStatementsApi.Services.Helpers
{
    /// <summary>
    /// CosmosDbRepository Helper class.
    /// </summary>
    public static class CosmosHelper
    {
        /// <summary>
        /// Get an instance of CosmosDbRepository.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the feature's services to.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A CosmosDbRepository.</returns>
        public static CosmosDbRepository GetCosmosDbRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var allocationsCosmosConfig = new AllocationsCosmosDbConfiguration();

            configuration.GetSection("AllocationsCosmosDb").Bind(allocationsCosmosConfig);

            var sp = services.BuildServiceProvider();
            var logger = sp.GetService<ILoggerAdapter<CosmosDbRepository>>()!;

            return new CosmosDbRepository(
                allocationsCosmosConfig.DatabaseName,
                allocationsCosmosConfig.CollectionName,
                allocationsCosmosConfig.ServiceEndpoint,
                allocationsCosmosConfig.AuthKeyOrResourceToken,
                logger,
                allocationsCosmosConfig.MaxRetryAttemptsOnThrottledRequests,
                allocationsCosmosConfig.MaxRetryWaitTimeInSeconds);
        }
    }
}