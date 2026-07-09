using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;

namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models
{
    /// <summary>
    /// An interface representing a single allocation returned from a search request on the Allocations API.
    /// </summary>
    public interface IAllocationApiSearchAllocation
    {
        /// <summary>
        /// The UKPRN of the provider.
        /// </summary>
        string Ukprn { get; set; }

        /// <summary>
        /// The type of provider.
        /// </summary>
        string ProviderType { get; set; }

        /// <summary>
        /// The sub-type of provider.
        /// </summary>
        string ProviderSubType { get; set; }

        /// <summary>
        /// The academic year of the allocation.
        /// </summary>
        int Year { get; set; }

        /// <summary>
        /// The version of this allocation.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// The provider name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The provider establishment number.
        /// </summary>
        string EstablishmentNumber { get; set; }

        /// <summary>
        /// The LA code.
        /// </summary>
        string LaCode { get; set; }

        /// <summary>
        /// The LA name.
        /// </summary>
        string LaName { get; set; }

        /// <summary>
        /// The provider postcode.
        /// </summary>
        string Postcode { get; set; }

        /// <summary>
        /// The provider town.
        /// </summary>
        string Town { get; set; }

        /// <summary>
        /// The date and time this allocation was published.
        /// </summary>
        DateTime PublishedDateTime { get; set; }

        /// <summary>
        /// The total amount of this allocation.
        /// </summary>
        decimal TotalAmount { get; set; }

        /// <summary>
        /// A collection of funding streams for the provider.
        /// </summary>
        IEnumerable<IFundingStream> Streams { get; set; }
    }
}
