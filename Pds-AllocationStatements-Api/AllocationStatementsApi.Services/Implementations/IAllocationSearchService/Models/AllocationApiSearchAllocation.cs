using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models
{
    /// <summary>
    /// A class representing a single allocation returned from a search request on the Allocations API.
    /// </summary>
    public class AllocationApiSearchAllocation : IAllocationApiSearchAllocation
    {
        /// <summary>
        /// The UKPRN of the provider.
        /// </summary>
        public string Ukprn { get; set; }

        /// <summary>
        /// The type of provider.
        /// </summary>
        public string ProviderType { get; set; }

        /// <summary>
        /// The sub-type of provider.
        /// </summary>
        public string ProviderSubType { get; set; }

        /// <summary>
        /// The academic year of the allocation.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The version of this allocation.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The provider name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The provider establishment number.
        /// </summary>
        public string EstablishmentNumber { get; set; }

        /// <summary>
        /// The LA code.
        /// </summary>
        public string LaCode { get; set; }

        /// <summary>
        /// The LA name.
        /// </summary>
        public string LaName { get; set; }

        /// <summary>
        /// The provider postcode.
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// The provider town.
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// The date and time this allocation was published.
        /// </summary>
        public DateTime PublishedDateTime { get; set; }

        /// <summary>
        /// The total amount of this allocation.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// A collection of funding streams for the provider.
        /// </summary>
        public IEnumerable<IFundingStream> Streams { get; set; }
    }
}