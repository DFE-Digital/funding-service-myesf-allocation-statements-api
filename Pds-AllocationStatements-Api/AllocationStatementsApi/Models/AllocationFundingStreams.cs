using AllocationStatementsApi.Enums;
using AllocationStatementsApi.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AllocationStatementsApi.Models
{
    /// <summary>
    /// The allocation funding stream.
    /// </summary>
    public class AllocationFundingStream
    {
        /// <summary>
        /// The title of the funding category.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the funding category.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The baseline of the funding stream.
        /// </summary>
        public decimal Baseline { get; set; }

        /// <summary>
        /// The first split of the total for the funding stream.
        /// </summary>
        public decimal SplitA { get; set; }

        /// <summary>
        /// The second split of the total for the funding stream.
        /// </summary>
        public decimal SplitB { get; set; }

        /// <summary>
        /// The learner support value for the funding stream.
        /// </summary>
        public decimal? LearnerSupportValue { get; set; }

        /// <summary>
        /// The total of the funding stream.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// The funding statement type.
        /// </summary>
        public AllocationFundingStreamType Type { get; set; }

        /// <summary>
        /// The allocation lines within the funding stream.
        /// </summary>
        public List<AllocationLine> AllocationLines { get; set; }

        /// <summary>
        /// Updated the allocation funding streams details for the UI.
        /// </summary>
        public AllocationFundingStream UpdateAllocationFundingStreamDetails(AllocationFundingStream allocationFundingStream)
        {
            var updatedAllocationFundingStreams = allocationFundingStream;
            if (Type != AllocationFundingStreamType.Unknown)
            {
                updatedAllocationFundingStreams.Title =
                    allocationFundingStream.Type.GetPropertyValue<AllocationFundingStreamType, DisplayAttribute, string>(o =>
                        o.Name);

                updatedAllocationFundingStreams.Description =
                    allocationFundingStream.Type.GetPropertyValue<AllocationFundingStreamType, DisplayAttribute, string>(o =>
                        o.Description);
            }

            return updatedAllocationFundingStreams;
        }
    }
}