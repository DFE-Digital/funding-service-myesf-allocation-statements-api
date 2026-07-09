using AllocationStatementsApi.Services.Helpers;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;
using Azure.Search.Documents;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService
{
    /// <summary>
    /// A class that wraps the Azure SearchIndexClient.
    /// </summary>
    public class AzureSearchIndexClient : IAzureSearchIndexClient
    {
        private readonly SearchClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchIndexClient"/> class.
        /// Create a new AzureSearchIndexClient.
        /// </summary>
        /// <param name="searchServiceName">The search service name.</param>
        /// <param name="indexName">The search index name.</param>
        /// <param name="queryApiKey">An API key that provides query-level access for the Azure Search service.</param>
        public AzureSearchIndexClient(string searchServiceName, string indexName, string queryApiKey)
        {
            _client = new SearchClient(searchServiceName.GetSearchServiceUri(), indexName, new Azure.AzureKeyCredential(queryApiKey));
        }

        /// <summary>
        /// Searches for documents in the Azure Search index.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="searchText">The search text.</param>
        /// <param name="parameters">The search parameters.</param>
        /// <returns>An object containing information about the result of the search.</returns>
        public async Task<List<T>> SearchDocumentsAsync<T>(string searchText, SearchOptions parameters)
            where T : class
        {
            var searchResults = await _client.SearchAsync<T>(searchText, parameters);
            return searchResults.Value.GetResults().Select(r => r.Document).ToList();
        }

        /// <summary>
        /// Searches for documents and LA Groups in the Azure Search index.
        /// </summary>
        /// <typeparam name="T">he type of documents to retrieve.</typeparam>
        /// <param name="searchText">The search text.</param>
        /// <param name="laGroupPropertyName">The LA Group Property Name.</param>
        /// <param name="parameters">The search parameters.</param>
        /// <returns>An object containing information about the result of the search.</returns>
        public async Task<object[]> SearchDocumentsAndLaGroups<T>(string searchText, string laGroupPropertyName, SearchOptions parameters)
            where T : class
        {
            var searchResults = await _client.SearchAsync<T>(searchText, parameters);

            var documents = searchResults.Value.GetResults().Select(r => r.Document).ToList();

            var laGroups = searchResults.Value.Facets[laGroupPropertyName].Select(g => g.Value.ToString()).ToList();

            return new object[] { documents, laGroups };
        }

        /// <summary>
        /// Gets the number of documents in the Azure Search index.
        /// </summary>
        /// <returns>The number of documents in the Azure Search index.</returns>
        public async Task<long> CountDocumentsAsync()
        {
            return await _client.GetDocumentCountAsync();
        }
    }
}