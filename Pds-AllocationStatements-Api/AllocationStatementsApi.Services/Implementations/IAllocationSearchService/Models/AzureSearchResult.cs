using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models
{
    /// <summary>
    /// A class representing the results of an Azure Search.
    /// </summary>
    public class AzureSearchResult<T> : ISearchResult<T>
    {
        /// <summary>
        /// The collection of matching documents.
        /// </summary>
        public IEnumerable<T> Documents { get; set; }
    }
}