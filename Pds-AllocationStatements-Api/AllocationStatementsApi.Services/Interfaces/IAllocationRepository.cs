using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;

namespace AllocationStatementsApi.Services.Interfaces
{
    /// <summary>
    /// The repository for allocations.
    /// This is also used in DataImportTool and Sfs tests, so any changes will also need to be amended in the monolith.
    /// </summary>
    public interface IAllocationRepository
    {
        /// <summary>
        /// Gets the allocation statement by identifier.
        /// </summary>
        /// <param name="id">The identifier. </param>
        /// <param name="ukprn">The ukprn. </param>
        /// <returns>Returns back the allocation.</returns>
        Task<List<IType>> GetAllocationStatementById(string id, string ukprn);

        /// <summary>
        /// Gets the allocation statements by ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn. </param>
        /// <returns>Returns back the allocations.</returns>
        Task<List<IType>> GetAllocationStatementsByUkprn(string ukprn);

        /// <summary>
        /// Gets back if the allocation statement exists for a given ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn. </param>
        /// <returns>Returns true if allocation exists for ukprn.</returns>
        Task<bool> GetAllocationStatementsExistsByUkprn(string ukprn);

        /// <summary>
        /// Gets the allocation statement not read for a ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn. </param>
        /// <param name="principal">The principal. </param>
        /// <returns>Returns back the allocations not read by a ukprn.</returns>
        Task<int> GetAllocationStatementsNotReadByUkprn(string ukprn, string principal);

        /// <summary>
        /// Gets the count of allocation statements by a range of CreatedAtDates.
        /// </summary>
        /// <param name="dateFrom">The date that you want the search range to start for CreatedAtDate. </param>
        /// <param name="dateTo">The date that you want the search range to end for CreatedAtDate. </param>
        /// <returns>Count of allocations.</returns>
        Task<int> GetAllocationStatementCountByCreatedAtDateRange(string dateFrom, string dateTo);

        /// <summary>
        /// Sets the allocation statement as read by identifier.
        /// </summary>
        /// <param name="id">The identifier. </param>
        /// <param name="ukprn">The ukprn. </param>
        /// <param name="principleId">The principleId. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetAllocationAsReadById(string id, string ukprn, string principleId);

        /// <summary>
        /// Adds the allocation by type.
        /// </summary>
        /// <param name="type">The type. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddAllocationStatement(IType type);

        /// <summary>
        /// Get the highest version number, or null if there are no current versions.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="ukprn">The ukprn. </param>
        /// <param name="year">The year.</param>
        /// <returns>The highest version number, or null.</returns>
        Task<IType> GetCurrentAllocationStatementByTypeAndYear(string type, string ukprn, int year);

        /// <summary>
        /// Clears down all allocation statements and history in cosmos db.
        /// </summary>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAllAllocationStatements();

        /// <summary>
        /// Clears down all allocation statements and history in cosmos db.
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAllAllocationStatements(string ukprn);

        /// <summary>
        /// Changes the throughput of the repository.
        /// </summary>
        /// <param name="rus">The rus.</param>
        /// <returns>A <see cref="Task{bool}"/> representing the asynchronous operation.</returns>
        Task<bool> ChangeThroughput(int rus);

        /// <summary>
        /// Gets the current throughput of the repository.
        /// </summary>
        /// <returns>A <see cref="Task{int}"/> representing the asynchronous operation.</returns>
        Task<int?> GetCurrentThroughput();
    }
}