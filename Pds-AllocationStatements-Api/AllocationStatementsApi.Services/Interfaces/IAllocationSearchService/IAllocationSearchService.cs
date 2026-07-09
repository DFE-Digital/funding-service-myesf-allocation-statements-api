using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;

namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService
{
    /// <summary>
    /// An interface providing methods for searching for allocations.
    /// </summary>
    public interface IAllocationSearchService
    {
        /// <summary>
        /// Search for a provider.
        /// </summary>
        /// <param name="allocationType">The allocation type to find.</param>
        /// <param name="searchTerm">The search term string to lookup.</param>
        /// <param name="beforeDate">Only find allocations published before this date and time.</param>
        /// <param name="year">The academic year to lookup.</param>
        /// <returns>A search result object containing all of the matching providers' allocations.</returns>
        Task<IAllocationSearchResult> SearchProvider(string allocationType, string searchTerm, DateTime beforeDate, int? year);

        /// <summary>
        /// Search for a Local Authority.
        /// </summary>
        /// <param name="allocationType">The allocation type to find.</param>
        /// <param name="searchTerm">The search term string to lookup.</param>
        /// <param name="beforeDate">Only find allocations published before this date and time.</param>
        /// <param name="year">The academic year to lookup.</param>
        /// <param name="customFilter">(Optional) A function that will be applied to filter the results that are returned.</param>
        /// <returns>A search result object containing the matching Local Authorities and some or all of their corresponding providers' allocations.</returns>
        Task<IAllocationSearchResult> SearchLocalAuthority(string allocationType, string searchTerm, DateTime beforeDate, int? year, Func<IAllocationSearchDocument, bool>? customFilter = null);
    }
}