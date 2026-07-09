using AllocationStatementsApi.Services.Configuration;
using AllocationStatementsApi.Services.Implementations.IAllocationSearchService;
using Microsoft.Extensions.Configuration;

namespace AllocationStatementsApi.Services.Helpers
{
    /// <summary>
    /// Azure Search Service helper class.
    /// </summary>
    public class SearchHelper
    {
        /// <summary>
        /// Gets the AzureAllocationSearchService.
        /// </summary>
        /// <param name="configuration">The Configuration.</param>
        /// <returns>a AzureAllocationSearchService.</returns>
        public static AzureAllocationSearchService GetAzureAllocationSearchService(IConfiguration configuration)
        {
            var azureSearchConfig = new AllocationsAzureSearchServiceConfiguration();

            configuration.GetSection("AllocationsAzureSearchService").Bind(azureSearchConfig);

            var providerSearchIndexName = azureSearchConfig.ProviderSearchIndexName;
            var localAuthoritySearchIndexName = azureSearchConfig.LocalAuthoritySearchIndexName;
            var laSearchFirstPageSize = azureSearchConfig.LocalAuthoritySearchFirstPageSize;

            var searchIndexClientManager = new AzureSearchIndexClientManager(
                    azureSearchConfig.Name,
                    azureSearchConfig.QueryApiKey,
                    providerSearchIndexName,
                    localAuthoritySearchIndexName);

            var laSearchFirstPageInt = int.TryParse(laSearchFirstPageSize, out int laSearch1stPageSize) ? laSearch1stPageSize : (int?)null;

            return new AzureAllocationSearchService(
                searchIndexClientManager,
                providerSearchIndexName,
                localAuthoritySearchIndexName,
                laSearchFirstPageInt);
        }
    }
}