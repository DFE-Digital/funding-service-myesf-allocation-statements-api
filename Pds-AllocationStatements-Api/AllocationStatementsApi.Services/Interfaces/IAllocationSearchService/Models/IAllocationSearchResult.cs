namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models
{
    /// <summary>
    /// An interface representing the result of an Allocation Search.
    /// </summary>
    public interface IAllocationSearchResult
    {
        /// <summary>
        /// Gets or sets the collection of matching documents.
        /// </summary>
        IEnumerable<IAllocationSearchDocument> Documents { get; set; }

        /// <summary>
        /// Gets or sets the collection of matching Local Authorities (JSON serialized).
        /// </summary>
        IEnumerable<string> LaGroups { get; set; }
    }
}