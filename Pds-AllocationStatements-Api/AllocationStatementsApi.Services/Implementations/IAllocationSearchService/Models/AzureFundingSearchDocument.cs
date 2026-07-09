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
    [AzureSearchIndex("fundings-funding")]
    public class AzureFundingSearchDocument : IFundingSearchDocument
    {
        /// <summary>
        /// Gets or sets the unique id for the funding.
        /// </summary>
        [JsonProperty("id")]
        [Key, SimpleField(IsFilterable = true, IsSortable = true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of grouping (e.g. LocalAuthoirty).
        /// </summary>
        [JsonProperty("groupingType")]
        [SimpleField(IsFilterable = true)]
        public string GroupingType { get; set; }

        /// <summary>
        /// Gets or sets the type of grouping (payment or information).
        /// </summary>
        [JsonProperty("groupingReason")]
        [SimpleField(IsFilterable = true)]
        public string GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the funding period code for this funding (e.g. AY-1920).
        /// </summary>
        [JsonProperty("fundingPeriodCode")]
        [SimpleField(IsFilterable = true)]
        public string FundingPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code (e.g. DSG).
        /// </summary>
        [JsonProperty("fundingStreamCode")]
        [SimpleField(IsFilterable = true)]
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets the date and time this allocation was published.
        /// </summary>
        [JsonProperty("statusChangedDate")]
        [SimpleField(IsFilterable = true)]
        public DateTime StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the version of this allocation.
        /// </summary>
        [JsonProperty("fundingVersion")]
        public string FundingVersion { get; set; }

        /// <summary>
        /// Gets or sets the version of the schema.
        /// </summary>
        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the version of the template.
        /// </summary>
        [JsonProperty("templateVersion")]
        public string TemplateVersion { get; set; }

        /// <summary>
        /// Gets or sets the group name (e.g. East Midlands).
        /// </summary>
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty("searchableGroupName")]
        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.StandardAsciiFoldingLucene)]
        public string SearchableGroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("groupUKPRN")]
        [SearchableField(IsHidden = false)]
        public string GroupUkprn { get; set; }

        /// <summary>
        /// Gets or sets a code to represent the code.
        /// </summary>
        [JsonProperty("groupCode")]
        [SearchableField(IsFilterable = true)]
        public string GroupCode { get; set; }

        /// <summary>
        /// Gets or sets the total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        public double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets Detail about the makeup of this funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        public string FundingValue { get; set; }
    }
}