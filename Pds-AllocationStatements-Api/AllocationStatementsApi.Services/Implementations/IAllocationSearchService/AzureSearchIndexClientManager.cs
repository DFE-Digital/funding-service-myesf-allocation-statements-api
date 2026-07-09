using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService
{
    /// <summary>
    /// A class for managing Azure Search Index Client connections.
    /// </summary>
    public class AzureSearchIndexClientManager : IAzureSearchIndexClientManager
    {
        private readonly string _searchServiceName, _queryApiKey;
        private IDictionary<string, IAzureSearchIndexClient> _searchIndexClients;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchIndexClientManager"/> class.
        /// Create a new instance of a AzureSearchIndexClientManager.
        /// </summary>
        /// <param name="searchServiceName">The name of the Azure Search service.</param>
        /// <param name="queryApiKey">The API key used for querying.</param>
        /// <param name="indexNames">The collection of index names for which to create clients.</param>
        public AzureSearchIndexClientManager(string searchServiceName, string queryApiKey, params string[] indexNames)
        {
            _searchServiceName = searchServiceName;
            _queryApiKey = queryApiKey;
            _searchIndexClients = new Dictionary<string, IAzureSearchIndexClient>();

            foreach (var indexName in indexNames)
            {
                _searchIndexClients.Add(
                    indexName,
                    new AzureSearchIndexClient(_searchServiceName, indexName, _queryApiKey));
            }
        }

        /// <summary>
        /// Get the Search Index Client for the given index.
        /// </summary>
        /// <param name="indexName">The index name to get.</param>
        /// <returns>A Search Index Client for the given index.</returns>
        public IAzureSearchIndexClient GetSearchIndexClient(string indexName)
        {
            if (!_searchIndexClients.ContainsKey(indexName))
            {
                throw new ArgumentOutOfRangeException(nameof(indexName), $"Search index {indexName} has not been configured.");
            }

            return _searchIndexClients[indexName];
        }
    }
}