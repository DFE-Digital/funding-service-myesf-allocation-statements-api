namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService
{
    /// <summary>
    /// An interface for managing Azure Search Index Client connections.
    /// </summary>
    public interface IAzureSearchIndexClientManager
    {
        /// <summary>
        /// Get the Search Index Client for the given index.
        /// </summary>
        /// <param name="indexName">The index name to get.</param>
        /// <returns>A Search Index Client for the given index.</returns>
        IAzureSearchIndexClient GetSearchIndexClient(string indexName);
    }
}