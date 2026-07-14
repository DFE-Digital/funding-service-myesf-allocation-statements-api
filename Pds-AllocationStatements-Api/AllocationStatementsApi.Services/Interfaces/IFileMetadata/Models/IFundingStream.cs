namespace AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models
{
    /// <summary>
    /// The funding stream allocation statement retrived from cosmsos db.
    /// </summary>
    public interface IFundingStream
    {
        /// <summary>
        /// Gets or sets the name of the funding stream.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the baseline amount of the funding stream.
        /// </summary>
        double? BaselineAmount { get; set; }

        /// <summary>
        /// Gets or sets the financial envelopes of the funding stream.
        /// </summary>
        IEnumerable<IFinancialEnvelope> Envelopes { get; set; }

        /// <summary>
        /// Gets or sets the allocation line of the funding stream.
        /// </summary>
        IEnumerable<ILine> Lines { get; set; }

        /// <summary>
        /// Gets or sets the total amount of the funding stream.
        /// </summary>
        double? TotalAmount { get; set; }
    }
}