using AllocationStatementsApi.Services.Enums;
using System;

namespace AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models
{
    /// <summary>
    /// The history of the allocation.
    /// This is also used in DataImportTool and Sfs tests, so any changes will also need to be amended in the monolith.
    /// </summary>
    public interface ITypeHistory
    {
        /// <summary>
        /// Gets or sets the type of action being recorded.
        /// </summary>
        FileAction Action { get; set; }

        /// <summary>
        /// Gets or sets the user the history relates to (if applicable).
        /// </summary>
        ITypeHistoryUser User { get; set; }

        /// <summary>
        /// Gets or sets when the event was raised.
        /// </summary>
        DateTime ActionDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets extra detail about the event being recorded.
        /// </summary>
        string Message { get; set; }
    }
}