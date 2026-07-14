namespace AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models
{
    /// <summary>
    /// The allocation statement at type level.
    /// This is also used in DataImportTool and Sfs tests, so any changes will also need to be amended in the monolith.
    /// </summary>
    public interface IType
    {
        /// <summary>
        /// Gets or sets the allocation id.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the allocation name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the allocation ukprn.
        /// </summary>
        string UKPRN { get; set; }

        /// <summary>
        /// Gets or sets the allocation funding streams.
        /// </summary>
        IEnumerable<IFundingStream> FundingStreams { get; set; }

        /// <summary>
        /// Gets or sets the allocation year.
        /// </summary>
        int Year { get; set; }

        /// <summary>
        /// Gets or sets the allocation version.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Gets or sets the allocation extra information.
        /// </summary>
        Dictionary<string, object> ExtraInfo { get; set; }

        /// <summary>
        /// Gets or sets the allocation history.
        /// </summary>
        IEnumerable<ITypeHistory> History { get; set; }

        /// <summary>
        /// Gets or sets the allocation total amount.
        /// </summary>
        double? TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets exclude the match (and previous versions) from the results.
        /// </summary>
        bool? ExcludeFromResults { get; set; }
    }
}