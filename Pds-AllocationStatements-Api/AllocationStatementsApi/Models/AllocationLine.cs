using AllocationStatementsApi.Enums;

namespace AllocationStatementsApi.Models
{
    public class AllocationLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllocationLine"/> class.
        /// Constructor.
        /// </summary>
        /// <param name="title">Allocation line title.</param>
        /// <param name="total">Allocation line total.</param>
        /// <param name="allocationLineType">Allocation line type.</param>
        /// <param name="displayOrder">Display order.</param>
        /// <param name="lineGroupOrder">Line group order.</param>
        public AllocationLine(string title, decimal total, AllocationLineType allocationLineType, int? displayOrder, int? lineGroupOrder)
        {
            Title = title;
            Total = total;
            Type = AllocationLineType.Unknown;
            DisplayOrder = displayOrder;
            LineGroupOrder = lineGroupOrder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllocationLine"/> class.
        /// Default constructor.
        /// </summary>
        public AllocationLine()
        {
        }

        /// <summary>
        /// The title of the allocation line.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The total of the allocation line.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// The type of allocation line
        /// </summary>
        public AllocationLineType Type { get; set; }

        /// <summary>
        /// Display order of allocation line.
        /// </summary>
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Allocation line group order.
        /// </summary>
        public int? LineGroupOrder { get; set; }
    }
}