using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models
{
    /// <summary>
    /// A class representing a document from an Azure Allocation Search index.
    /// </summary>
    public class AzureAllocationSearchDocument : IAllocationSearchDocument
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
        /// A collection of funding streams for the provider (JSON serialized).
        /// </summary>
        public string[] Streams { get; set; }

        /// <summary>
        /// The academic year of the allocation.
        /// </summary>
        [JsonProperty("year")] // We need this JsonProperty (even though it only differs from the member name by case) to make the azure filter work.
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
        [JsonProperty("LocalAuthority")]
        public string LaName { get; set; }

        /// <summary>
        /// The Local Authority (JSON serialized).
        /// </summary>
        public string LaGroup { get; set; }

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
        [JsonProperty("PublishedDate")]
        public DateTime PublishedDateTime { get; set; }

        /// <summary>
        /// The total amount of this allocation.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// If true, this provider should not be returned even if a previous version exists with this property set to false.
        /// </summary>
        public bool ExcludeFromResults { get; set; }
    }
}