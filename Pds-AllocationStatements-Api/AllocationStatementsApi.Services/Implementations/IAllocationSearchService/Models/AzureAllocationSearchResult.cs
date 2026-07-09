using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models
{
    /// <summary>
    /// A class representing the result of an Azure Allocation Search.
    /// </summary>
    public class AzureAllocationSearchResult : IAllocationSearchResult
    {
        /// <summary>
        /// The collection of matching documents.
        /// </summary>
        public IEnumerable<IAllocationSearchDocument> Documents { get; set; }

        /// <summary>
        /// The collection of matching Local Authorities (JSON serialized).
        /// </summary>
        public IEnumerable<string> LaGroups { get; set; }
    }
}
