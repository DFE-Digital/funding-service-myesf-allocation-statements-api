using AllocationStatementsApi.Services.Helpers;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Implementations.IFileMetadata.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// The allocation at type level.
    /// </summary>
    public class Type : IType
    {
        /// <summary>
        /// Gets or sets cosmosDB document needs a property called 'id' to work as a primary key.
        /// </summary>
        [JsonProperty(PropertyName = "id")]

        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the allocation statement.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the ukprn of the allocation statement. Made this a string as CosmosDB protal doesnt like none-string partition keys.
        /// </summary>
        [JsonProperty(PropertyName = "ukprn")]
        public string UKPRN { get; set; }

        /// <summary>
        /// Gets or sets the funding streams of an allocation statement.
        /// </summary>
        [JsonProperty(PropertyName = "streams"), JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<FundingStream>>))]
        public IEnumerable<IFundingStream> FundingStreams { get; set; }

        /// <summary>
        /// Gets or sets the year of a allocation statement.
        /// </summary>
        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the version of a allocation statement.
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the extra information of a allocation statement.
        /// </summary>
        [JsonProperty(PropertyName = "extraInfo")]
        public Dictionary<string, object> ExtraInfo { get; set; }

        /// <summary>
        /// Gets or sets the history of the file (such as when the allocation was last viewed via the portal (if ever)).
        /// </summary>
        [JsonProperty(PropertyName = "history"), JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<TypeHistory>>))]
        public IEnumerable<ITypeHistory> History { get; set; }

        /// <summary>
        /// Gets or sets the total amount of the allocation statement.
        /// </summary>
        [JsonProperty(PropertyName = "totalAmount")]
        public double? TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets exclude the match (and previous versions) from the results.
        /// </summary>
        [JsonProperty(PropertyName = "excludeFromResults")]
        public bool? ExcludeFromResults { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}