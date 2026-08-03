using AllocationStatementsApi.Enums;

namespace AllocationStatementsApi.Models
{
    /// <summary>
    /// The allocation statement.
    /// </summary>
    public class AllocationStatement
    {
        /// <summary>
        /// Gets or sets the data store id of this instance.
        /// </summary>
        public  required string Id { get; set; }

        /// <summary>
        /// Gets or sets the period of the funding statement.
        /// </summary>
        public required string Period { get; set; }

        /// <summary>
        /// Gets or sets the version number of the funding statement.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the Ukprn of the funding statement.
        /// </summary>
        public required string Ukprn { get; set; }

        /// <summary>
        /// Gets or sets the funding statement type.
        /// </summary>
        public FundingStatementType Type { get; set; }

        /// <summary>
        /// Gets or sets the date the funding statement has been created.
        /// </summary>
        public DateTime CreatedAtDate { get; set; }

        /// <summary>
        /// Gets or sets the total value of the funding statement.
        /// </summary>
        public decimal TotalValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is true if the allocation statement has been read.
        /// </summary>
        public bool HasBeenRead { get; set; }

        /// <summary>
        /// Gets or sets the funding streams within the funding statement.
        /// </summary>
        public required List<AllocationFundingStream> AllocationFundingStreams { get; set; }
    }
}