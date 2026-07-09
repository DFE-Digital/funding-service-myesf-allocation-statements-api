namespace AllocationStatementsApi.Models
{
    public class AllocationViewedInfo
    {
        /// <summary>
        /// Gets or sets the Id of the allocation that has been viewed'.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the Ukprn of the allocation that has been viewed'.
        /// </summary>
        public string ukprn { get; set; }

        /// <summary>
        /// Gets or sets the Principal id of the user that has viewed the allocation.
        /// </summary>
        public string principleId { get; set; }
    }
}