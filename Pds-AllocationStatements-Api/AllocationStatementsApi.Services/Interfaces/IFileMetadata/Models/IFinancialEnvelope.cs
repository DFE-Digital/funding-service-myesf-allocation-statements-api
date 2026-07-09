namespace AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models
{
    /// <summary>
    /// The financial envelopes for the funding statement.
    /// This is also used in DataImportTool and Sfs tests, so any changes will also need to be amended in the monolith.
    /// </summary>
    public interface IFinancialEnvelope
    {
        /// <summary>
        /// Gets or sets name of the financial envelope.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount of the financial envelope.
        /// </summary>
        double? Amount { get; set; }
    }
}