using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models
{
    /// <summary>
    /// An interface representing basic information about a Local Authority.
    /// </summary>
    public interface ILocalAuthority
    {
        /// <summary>
        /// The LA code.
        /// </summary>
        [JsonProperty("Name")]
        string LaCode { get; set; }

        /// <summary>
        /// The LA name.
        /// </summary>
        [JsonProperty("LocalAuthority")]
        string LaName { get; set; }
    }
}