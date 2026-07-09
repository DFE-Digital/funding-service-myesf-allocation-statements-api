using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Implementations.IFileMetadata.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// The user information of history audits.
    /// </summary>
    public class TypeHistoryUser : ITypeHistoryUser
    {
        /// <summary>
        /// Gets or sets user principle.
        /// </summary>
        [JsonProperty(PropertyName = "principle")]
        public string Principle { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}