namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models
{
    /// <summary>
    /// An interface representing the result of a search.
    /// </summary>
    /// <typeparam name="T">The type of the search result documents.</typeparam>
    public interface ISearchResult<T>
    {
        /// <summary>
        /// The collection of matching documents.
        /// </summary>
        IEnumerable<T> Documents { get; set; }
    }
}