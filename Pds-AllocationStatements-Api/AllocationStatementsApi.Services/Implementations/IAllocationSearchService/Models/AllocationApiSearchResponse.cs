using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using ILocalAuthority = AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models.ILocalAuthority;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models
{
    /// <summary>
    /// A class representing the response from a search request on the Allocations API.
    /// </summary>
    public class AllocationApiSearchResponse : IAllocationApiSearchResponse
    {
        /// <summary>
        /// Gets or sets the collection of matching allocations.
        /// </summary>
        public IEnumerable<IAllocationApiSearchAllocation>? Allocations { get; set; }

        /// <summary>
        /// Gets or sets the collection of matching Local Authorities.
        /// </summary>
        public IEnumerable<ILocalAuthority>? LocalAuthorities { get; set; }
    }
}
