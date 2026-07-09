using Azure.Search.Documents;

namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService
{
    /// <summary>
    /// An interface that exposes methods for querying an Azure Search index.
    /// </summary>
    public interface IAzureSearchIndexClient
    {
        /// <summary>
        /// Perform an Azure Search.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="searchText">The search text.</param>
        /// <param name="parameters">The search parameters.</param>
        /// <returns>An object containing information about the result of the search.</returns>
        Task<List<T>> SearchDocumentsAsync<T>(string searchText, SearchOptions parameters)
            where T : class;

        /// <summary>
        /// Searches for documents and LA Groups in the Azure Search index.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="searchText">The search text.</param>
        /// <param name="laGroupPropertyName">The LA Group Property Name.</param>
        /// <param name="parameters">The search parameters.</param>
        /// <returns>An object containing information about the result of the search.</returns>
        Task<object[]> SearchDocumentsAndLaGroups<T>(string searchText, string laGroupPropertyName, SearchOptions parameters)
            where T : class;

        /// <summary>
        /// Gets the number of documents in the Azure Search index.
        /// </summary>
        /// <returns>The number of documents in the Azure Search index.</returns>
        Task<long> CountDocumentsAsync();
    }
}