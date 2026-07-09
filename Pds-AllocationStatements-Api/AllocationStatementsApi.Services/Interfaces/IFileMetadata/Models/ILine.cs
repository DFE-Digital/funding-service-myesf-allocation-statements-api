namespace AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models
{
    /// <summary>
    /// The allocation line of the allocation statement.
    /// This is also used in DataImportTool and Sfs tests, so any changes will also need to be amended in the monolith.
    /// </summary>
    public interface ILine
    {
        /// <summary>
        /// Gets or sets the name of the allocation line.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount of the allocation line.
        /// </summary>
        double? Amount { get; set; }
    }
}