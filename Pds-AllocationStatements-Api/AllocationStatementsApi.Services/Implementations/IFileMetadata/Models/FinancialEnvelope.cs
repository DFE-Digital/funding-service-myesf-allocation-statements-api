using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Implementations.IFileMetadata.Models
{
    /// <summary>
    /// The financial envelope of a funding stream.
    /// </summary>
    public class FinancialEnvelope : IFinancialEnvelope
    {
        /// <summary>
        /// The name of the financial envelope.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The amount of the financial envelope.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public double? Amount { get; set; }
    }
}