using Newtonsoft.Json;
using System;

namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models
{
    /// <summary>
    /// An interface representing a document from a Provider Funding Search.
    /// </summary>
    public interface IProviderFundingSearchDocument
    {
        /// <summary>
        /// The unique id for the funding.
        /// </summary>
        [JsonProperty("id")]
        string Id { get; set; }

        /// <summary>
        /// The ID of the funding this came from (if there are multiple, there will be multiple instances of this object).
        /// </summary>
        [JsonProperty("parentId")]
        string ParentId { get; set; }

        /// <summary>
        /// The funding period code for this funding (e.g. AY-1920).
        /// </summary>
        [JsonProperty("fundingPeriodCode")]
        string FundingPeriodCode { get; set; }

        /// <summary>
        /// The funding stream code (e.g. DSG).
        /// </summary>
        [JsonProperty("fundingStreamCode")]
        string FundingStreamCode { get; set; }

        /// <summary>
        /// The date and time this allocation was published.
        /// </summary>
        [JsonProperty("statusChangedDate")]
        DateTime StatusChangedDate { get; set; }

        /// <summary>
        /// The version of this allocation.
        /// </summary>
        [JsonProperty("fundingVersion")]
        string FundingVersion { get; set; }

        /// <summary>
        /// The version of the schema.
        /// </summary>
        [JsonProperty("schemaVersion")]
        string SchemaVersion { get; set; }

        /// <summary>
        /// The version of the template.
        /// </summary>
        [JsonProperty("templateVersion")]
        string TemplateVersion { get; set; }

        /// <summary>
        /// The provider name.
        /// </summary>
        [JsonProperty("organisationName")]
        string OrganisationName { get; set; }

        /// <summary>
        /// The provider name.
        /// </summary>
        [JsonProperty("searchableOrganisationName")]
        string SearchableOrganisationName { get; set; }

        /// <summary>
        /// The provider establishment number.
        /// </summary>
        [JsonProperty("organisationUkprn")]
        string OrganisationUkprn { get; set; }

        /// <summary>
        /// The provider establishment number.
        /// </summary>
        [JsonProperty("organisationDfeNumber")]
        string OrganisationDfeNumber { get; set; }

        /// <summary>
        /// The provider town.
        /// </summary>
        [JsonProperty("organisationTown")]
        string OrganisationTown { get; set; }

        /// <summary>
        /// The provider postcode.
        /// </summary>
        [JsonProperty("organisationPostcode")]
        string OrganisationPostcode { get; set; }

        /// <summary>
        /// Name of the parent organisation group.
        /// </summary>
        [JsonProperty("parentName")]
        string ParentName { get; set; }

        /// <summary>
        /// The parent organisation group's primary identifier (e.g. UKPRN).
        /// </summary>
        [JsonProperty("parentPrimaryIdentifier")]
        string ParentPrimaryIdentifier { get; set; }

        /// <summary>
        /// The parent organisation groups' provider type (e.g. LocalAuthority).
        /// </summary>
        [JsonProperty("parentProviderType")]
        string ParentProviderType { get; set; }

        /// <summary>
        /// The type of provider.
        /// </summary>
        [JsonProperty("providerType")]
        string ProviderType { get; set; }

        /// <summary>
        /// The sub-type of provider.
        /// </summary>
        [JsonProperty("providerSubType")]
        string ProviderSubType { get; set; }

        /// <summary>
        /// The status of provider (e.g. 'Open').
        /// </summary>
        [JsonProperty("providerStatus")]
        string ProviderStatus { get; set; }

        /// <summary>
        /// The closure reason (e.g. 'Not applicable').
        /// </summary>
        [JsonProperty("closeReason")]
        string CloseReason { get; set; }

        /// <summary>
        /// The total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        double TotalAmount { get; set; }

        /// <summary>
        /// Detail about the makeup of this provider funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        string FundingValue { get; set; }

        /// <summary>
        /// The grouping reason (e.g. Payment or Information).
        /// </summary>
        [JsonProperty("groupingReason")]
        string GroupingReason { get; set; }
    }
}
