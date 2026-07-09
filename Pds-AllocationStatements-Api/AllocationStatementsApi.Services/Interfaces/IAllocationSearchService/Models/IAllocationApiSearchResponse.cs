namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models
{
    /// <summary>
    /// An interface representing the response from a search request on the Allocations API.
    /// </summary>
    public interface IAllocationApiSearchResponse
    {
        /// <summary>
        /// Gets or sets the collection of matching allocations.
        /// </summary>
        IEnumerable<IAllocationApiSearchAllocation>? Allocations { get; set; }

        /// <summary>
        /// Gets or sets the collection of matching Local Authorities.
        /// </summary>
        IEnumerable<ILocalAuthority>? LocalAuthorities { get; set; }
    }
}