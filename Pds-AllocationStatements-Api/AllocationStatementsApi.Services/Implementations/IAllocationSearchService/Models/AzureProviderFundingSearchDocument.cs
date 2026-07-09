using AllocationStatementsApi.Services.Attributes;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models
{
    /// <summary>
    /// A class representing a document from an Azure Funding Search index.
    /// </summary>
    [AzureSearchIndex("fundings-providerfunding")]
    public class AzureProviderFundingSearchDocument : IProviderFundingSearchDocument
    {
        /// <summary>
        /// The unique id for the funding.
        /// </summary>
        [JsonProperty("uniqueid")]
        [Key, SimpleField(IsFilterable = true, IsSortable = true)]
        public string UniqueId { get; set; }

        /// <summary>
        /// The unique id for the funding.
        /// </summary>
        [JsonProperty("id")]
        [SimpleField(IsFilterable = true)]
        public string Id { get; set; }

        /// <summary>
        /// The Id of the funding this came from (if there are multiple, there will be multiple instances of this object).
        /// </summary>
        [JsonProperty("parentId")]
        [SimpleField(IsFilterable = true)]
        public string ParentId { get; set; }

        /// <summary>
        /// The funding period code for this funding (e.g. AY-1920).
        /// </summary>
        [JsonProperty("fundingPeriodCode")]
        [SimpleField(IsFilterable = true)]
        public string FundingPeriodCode { get; set; }

        /// <summary>
        /// The funding stream code (e.g. DSG).
        /// </summary>
        [JsonProperty("fundingStreamCode")]
        [SimpleField(IsFilterable = true)]
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// The date and time this allocation was published.
        /// </summary>
        [JsonProperty("statusChangedDate")]
        [SimpleField(IsFilterable = true)]
        public DateTime StatusChangedDate { get; set; }

        /// <summary>
        /// The version of this allocation.
        /// </summary>
        [JsonProperty("fundingVersion")]
        public string FundingVersion { get; set; }

        /// <summary>
        /// The version of the schema.
        /// </summary>
        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; set; }

        /// <summary>
        /// The version of the template.
        /// </summary>
        [JsonProperty("templateVersion")]
        public string TemplateVersion { get; set; }

        /// <summary>
        /// The provider name.
        /// </summary>
        [JsonProperty("organisationName")]
        public string OrganisationName { get; set; }

        /// <summary>
        /// The searchable provider name (strips some characters).
        /// </summary>
        [JsonProperty("searchableOrganisationName")]
        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.StandardAsciiFoldingLucene)]
        public string SearchableOrganisationName { get; set; }

        /// <summary>
        /// The provider establishment number.
        /// </summary>
        [JsonProperty("organisationUkprn")]
        [SearchableField(IsFilterable = true)]
        public string OrganisationUkprn { get; set; }

        /// <summary>
        /// The provider establishment number.
        /// </summary>
        [JsonProperty("organisationDfeNumber")]
        [SearchableField(IsHidden = false)]
        public string OrganisationDfeNumber { get; set; }

        /// <summary>
        /// The provider town.
        /// </summary>
        [JsonProperty("organisationTown")]
        public string OrganisationTown { get; set; }

        /// <summary>
        /// The provider postcode.
        /// </summary>
        [JsonProperty("organisationPostcode")]
        public string OrganisationPostcode { get; set; }

        /// <summary>
        /// Name of the parent organisation group.
        /// </summary>
        [JsonProperty("parentName")]
        public string ParentName { get; set; }

        /// <summary>
        /// The parent organisation group's primary identifier (e.g. UKPRN).
        /// </summary>
        [JsonProperty("parentPrimaryIdentifier")]
        [SimpleField(IsFilterable = true)]
        public string ParentPrimaryIdentifier { get; set; }

        /// <summary>
        /// The parent organisation groups' provider type (e.g. LocalAuthority).
        /// </summary>
        [JsonProperty("parentProviderType")]
        public string ParentProviderType { get; set; }

        /// <summary>
        /// The type of provider.
        /// </summary>
        [JsonProperty("providerType")]
        public string ProviderType { get; set; }

        /// <summary>
        /// The status of provider (e.g. 'Open').
        /// </summary>
        [JsonProperty("providerStatus")]
        [SimpleField(IsFilterable = true)]
        public string ProviderStatus { get; set; }

        /// <summary>
        /// The closure reason (e.g. 'Not applicable').
        /// </summary>
        [JsonProperty("closeReason")]
        public string CloseReason { get; set; }

        /// <summary>
        /// The sub-type of provider.
        /// </summary>
        [JsonProperty("providerSubType")]
        public string ProviderSubType { get; set; }

        /// <summary>
        /// The total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        public double TotalAmount { get; set; }

        /// <summary>
        /// Detail about the makeup of this provider funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        public string FundingValue { get; set; }

        /// <summary>
        /// The grouping reason (e.g. Payment or Information).
        /// </summary>
        [JsonProperty("groupingReason")]
        [SimpleField(IsFilterable = true)]
        public string GroupingReason { get; set; }
    }
}