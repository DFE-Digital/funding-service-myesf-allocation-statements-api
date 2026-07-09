using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Implementations.IFileMetadata.Models
{
    /// <summary>
    /// The allocation line retrived from cosmos db.
    /// </summary>
    public class Line : ILine
    {
        /// <summary>
        /// The name of the allocation line.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The amount of the allocation line.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public double? Amount { get; set; }
    }
}