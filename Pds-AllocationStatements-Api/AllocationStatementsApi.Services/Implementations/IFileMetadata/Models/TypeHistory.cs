using AllocationStatementsApi.Services.Enums;
using AllocationStatementsApi.Services.Helpers;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AllocationStatementsApi.Services.Implementations.IFileMetadata.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// The history of the allocation.
    /// </summary>
    public class TypeHistory : ITypeHistory
    {
        /// <summary>
        /// Gets or sets the type of action being recorded.
        /// </summary>
        [JsonProperty(PropertyName = "action"), JsonConverter(typeof(StringEnumConverter))]
        public FileAction Action { get; set; }

        /// <summary>
        /// Gets or sets the user the history relates to (if applicable).
        /// </summary>
        [JsonProperty(PropertyName = "user"), JsonConverter(typeof(ConcreteTypeConverter<TypeHistoryUser>))]
        public ITypeHistoryUser User { get; set; }

        /// <summary>
        /// Gets or sets when the event was raised.
        /// </summary>
        [JsonProperty(PropertyName = "actionDateTimeUtc")]
        public DateTime ActionDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets extra detail about the event being recorded.
        /// </summary>
        [JsonProperty(PropertyName = "message")]

        public string Message { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}