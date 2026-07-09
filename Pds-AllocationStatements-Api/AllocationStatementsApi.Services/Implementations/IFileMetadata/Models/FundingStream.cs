using AllocationStatementsApi.Services.Helpers;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Implementations.IFileMetadata.Models
{
    /// <summary>
    /// The funding stream of a allocation statement.
    /// </summary>
    public class FundingStream : IFundingStream
    {
        /// <summary>
        /// The name of the funding stream.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The baseline of the funding stream.
        /// </summary>
        [JsonProperty(PropertyName = "baselineAmount")]
        public double? BaselineAmount { get; set; }

        /// <summary>
        /// The financial envelopes of the funding stream.
        /// </summary>
        [JsonProperty(PropertyName = "envelopes"), JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<FinancialEnvelope>>))]
        public IEnumerable<IFinancialEnvelope> Envelopes { get; set; }

        /// <summary>
        /// The lines of the of the funding statement.
        /// </summary>
        [JsonProperty(PropertyName = "lines"), JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<Line>>))]
        public IEnumerable<ILine> Lines { get; set; }

        /// <summary>
        /// The total amount of the funding stream.
        /// </summary>
        [JsonProperty(PropertyName = "totalAmount")]
        public double? TotalAmount { get; set; }
    }
}