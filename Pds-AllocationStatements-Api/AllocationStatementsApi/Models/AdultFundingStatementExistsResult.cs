namespace AllocationStatementsApi.Models
{
    /// <summary>
    /// Allocation statements exist result model.
    /// </summary>
    public class AdultFundingStatementExistsResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether allocation statements exist for the given ukprn.
        /// </summary>
        public bool AllocationExists { get; set; }

        /// <summary>
        /// Gets or sets the number of unread new allocations.
        /// </summary>
        public int UnreadNewAllocations { get; set; }

        /// <summary>
        /// Gets or sets the number of unread updated allocations.
        /// </summary>
        public int UnreadUpdatedAllocations { get; set; }
    }
}