using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using Newtonsoft.Json;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models
{
    /// <summary>
    /// A class representing basic information about a Local Authority.
    /// </summary>
    public class LocalAuthority : ILocalAuthority
    {
        /// <summary>
        /// The LA code.
        /// </summary>
        [JsonProperty("LaCode")]
        public string LaCode { get; set; }

        /// <summary>
        /// The LA name.
        /// </summary>
        [JsonProperty("LaName")]
        public string LaName { get; set; }
    }
}