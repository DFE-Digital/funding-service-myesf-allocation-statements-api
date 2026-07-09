using AllocationStatementsApi.Enums;
using AllocationStatementsApi.Helpers;
using AllocationStatementsApi.Models;
using AllocationStatementsApi.Services.Enums;
using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Interfaces;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pds.Core.Logging;
using System.ComponentModel.DataAnnotations;

namespace AllocationStatementsApi.Controllers
{
    /// <summary>
    /// Allocations api controller.
    /// </summary>
    [ApiController]
    public class AllocationsController : Controller
    {
        private readonly IAllocationRepository _allocationRepository;
        private readonly IAllocationSearchService _allocationSearchService;
        private readonly ILoggerAdapter<AllocationsController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllocationsController"/> class.
        /// Constructor for the allocations controller.
        /// </summary>
        /// <param name="allocationRepository">The repository containing allocations.</param>
        /// <param name="allocationSearchService">The service to use for searching for allocations.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper for mapping objects.</param>
        public AllocationsController(IAllocationRepository allocationRepository, IAllocationSearchService allocationSearchService, ILoggerAdapter<AllocationsController> logger, IMapper mapper)
        {
            _allocationRepository = allocationRepository;
            _allocationSearchService = allocationSearchService;
            _logger = logger;
            _mapper = mapper;
        }

        #region Public actions

        /// <summary>
        /// Get allocations by ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>A list of allocation statements.</returns>
        [HttpGet("api/[controller]/GetByUkprn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<AdultFundingStatementsResult> GetByUkprn(string ukprn, string principal)
        {
            _logger.LogInformation($"Get allocation statements by ukprn called with id: {ukprn}.");

            var allocations = await _allocationRepository.GetAllocationStatementsByUkprn(ukprn);

            var result = new AdultFundingStatementsResult
            {
                AllocationStatements = allocations.Select(allocation => new AllocationStatement
                {
                    CreatedAtDate = GetDateTime(allocation.History) ?? DateTime.MinValue,
                    Id = allocation.Id,
                    Period = allocation.Year.ToString(),
                    TotalValue = Convert.ToDecimal(allocation.TotalAmount ?? -1),
                    Type = GetType(allocation.Name, allocation.Year),
                    Ukprn = allocation.UKPRN,
                    Version = allocation.Version,
                    AllocationFundingStreams = allocation.FundingStreams.Select(GetAllocationFundingStream).ToList(),
                    HasBeenRead = allocation.History?.Any(x => x.Action == FileAction.ViewedInDetail && x.User.Principle == principal) ?? false
                }).Where(a => a.Type != FundingStatementType.Other).ToList()
            };

            return result;
        }

        /// <summary>
        /// Get count of allocations within a date range.
        /// </summary>
        /// <param name="dateFrom">The date to begin search from.</param>
        /// <param name="dateTo">The date to end search on.</param>
        /// <returns>A count of allocation statements.</returns>
        [HttpGet("api/[controller]/GetCountByCreatedAtDateRange")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<int> GetCountByCreatedAtDateRange(string dateFrom, string dateTo)
        {
            _logger.LogInformation($"Get allocation statement count by created at date range called with date range: {dateFrom} - {dateTo}.");

            return await _allocationRepository.GetAllocationStatementCountByCreatedAtDateRange(dateFrom, dateTo);
        }

        /// <summary>
        /// Gets an allocation by id.
        /// </summary>
        /// <param name="id">The statement's Id.</param>
        /// <param name="ukprn">The provider's UKPRN.</param>
        /// <returns>Returns back an allocation.</returns>
        [HttpGet("api/[controller]/GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<AdultFundingStatementResult> GetById(string id, string ukprn)
        {
            _logger.LogInformation($"Get allocation statement by id called with id: {id} and ukprn: {ukprn}.");

            var allocationResult = await _allocationRepository.GetAllocationStatementById(id, ukprn);

            var allocation = allocationResult.First();

            return new AdultFundingStatementResult
            {
                AllocationStatement = new AllocationStatement
                {
                    CreatedAtDate = GetDateTime(allocation.History) ?? DateTime.MinValue,
                    Id = allocation.Id,
                    Period = allocation.Year.ToString(),
                    TotalValue = Convert.ToDecimal(allocation.TotalAmount ?? -1),
                    Type = GetType(allocation.Name, allocation.Year),
                    Ukprn = allocation.UKPRN,
                    Version = allocation.Version,
                    AllocationFundingStreams = allocation.FundingStreams.Select(GetAllocationFundingStream).ToList()
                }
            };
        }

        /// <summary>
        /// Returns back if allocations exist and have been read for a given ukprn.
        /// </summary>
        /// <param name="ukprn">The provider's UKPRN.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>If the allocation exists for ukprn and how many allocations have not been read.</returns>
        [HttpGet("api/[controller]/GetExistsByUkprn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<AdultFundingStatementExistsResult> GetExistsByUkprn(string ukprn, string principal)
        {
            _logger.LogInformation($"Get allocation statement by ukprn called with ukprn: {ukprn} and principal: {principal}.");

            var allocationsForUkprn = await GetByUkprn(ukprn, principal);

            var allocationExists = allocationsForUkprn?.AllocationStatements?.Any() == true;

            var unreadNewAllocationsCount = !allocationExists
                ? 0
                : (allocationsForUkprn !).AllocationStatements.Count(a => !a.HasBeenRead && a.Version == 1);

            var unreadUpdatedAllocationsCount = !allocationExists
                ? 0
                : (allocationsForUkprn !).AllocationStatements.Count(a => !a.HasBeenRead && a.Version > 1);

            // TODO: Current code is a workaround. Ideally make the below sprocs accept a list of accepted allocation types and filter on that list.
            //var allocationExists = await _allocationRepository.GetAllocationStatementsExistsByUkprn(ukprn);
            //var allocationNotReadCount = await _allocationRepository.GetAllocationStatementsNotReadByUkprn(ukprn, principal);
            return new AdultFundingStatementExistsResult
            {
                AllocationExists = allocationExists,
                UnreadNewAllocations = unreadNewAllocationsCount,
                UnreadUpdatedAllocations = unreadUpdatedAllocationsCount
            };
        }

        /// <summary>
        /// Sets allocations as read by id.
        /// </summary>
        /// <param name="allocationInfo">The allocation information that has been posted.</param>
        /// <returns>If the allocation exists for ukprn and how many allocations have not been read.</returns>
        [HttpPost("api/[controller]/SetAllocationAsReadById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<bool> SetAllocationAsReadById([FromBody] AllocationViewedInfo allocationInfo)
        {
            _logger.LogInformation($"Set allocation as read by id called with id: {allocationInfo.id},  ukprn: {allocationInfo.ukprn}, and principleId: {allocationInfo.principleId}.");

            await _allocationRepository.SetAllocationAsReadById(allocationInfo.id, allocationInfo.ukprn, allocationInfo.principleId);

            return true;
        }

        /// <summary>
        /// Search for allocations by provider details.
        /// </summary>
        /// <param name="allocationType">(Not used) The type of allocations to lookup.</param>
        /// <param name="searchTerm">The search term string to match.</param>
        /// <param name="beforeDateTime">Only find allocations published before this date and time.</param>
        /// <param name="year">The academic year to lookup.</param>
        /// <returns>A response object containing the allocations for the matching provider(s).</returns>
        [HttpGet("api/[controller]/SearchProvider")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IAllocationApiSearchResponse> SearchProvider(FundingStatementType allocationType, string searchTerm, DateTime beforeDateTime, int year)
        {
            _logger.LogInformation($"Search provider called with allocation type: {allocationType},  search term: {searchTerm}, before datetime: {beforeDateTime}, and year: {year}.");

            var searchResult =
                await _allocationSearchService.SearchProvider(allocationType.ToString(), searchTerm, beforeDateTime, year);

            return GetApiSearchResponse(searchResult) !;
        }

        /// <summary>
        /// Search for allocations by Local Authority details.
        /// </summary>
        /// <param name="allocationType">(Not used) The type of allocations to lookup.</param>
        /// <param name="searchTerm">The search term string to match.</param>
        /// <param name="beforeDateTime">Only find allocations published before this date and time.</param>
        /// <param name="year">The academic year to lookup.</param>
        /// <returns>A response object containing the list of matching LAs. If only one LA is found, the response also contains all of the allocations for providers under that LA.</returns>
        [HttpGet("api/[controller]/SearchLocalAuthority")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IAllocationApiSearchResponse> SearchLocalAuthority(FundingStatementType allocationType, string searchTerm, DateTime beforeDateTime, int year)
        {
            _logger.LogInformation($"Search local authority called with funding statement type: {allocationType},  search term: {searchTerm}, before datetime: {beforeDateTime}, and year: {year}.");

            var searchResult =
                await _allocationSearchService.SearchLocalAuthority(allocationType.ToString(), searchTerm, beforeDateTime, year);

            return GetApiSearchResponse(searchResult) !;
        }

        /// <summary>
        /// Find allocations for the given Local Authority.
        /// </summary>
        /// <param name="allocationType">(Not used) The type of allocations to lookup.</param>
        /// <param name="laCode">The LA code to lookup.</param>
        /// <param name="beforeDateTime">Only find allocations published before this date and time.</param>
        /// <param name="year">The academic year to lookup.</param>
        /// <returns>A response object containing the allocations for all providers under the given Local Authority.</returns>
        [HttpGet("api/[controller]/SearchLocalAuthorityByCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IAllocationApiSearchResponse> SearchLocalAuthorityByCode(FundingStatementType allocationType, int laCode, DateTime beforeDateTime, int year)
        {
            _logger.LogInformation($"Search local authority by code called with funding statement type: {allocationType},  LA Code: {laCode}, before datetime: {beforeDateTime}, and year: {year}.");

            var laCodeString = laCode.ToString();

            var searchResult = await _allocationSearchService.SearchLocalAuthority(
                    allocationType.ToString(),
                    laCodeString,
                    beforeDateTime,
                    year,
                    new Func<IAllocationSearchDocument, bool>(d => d.LaCode == laCodeString));

            return GetApiSearchResponse(searchResult, laCode) !;
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Gets the allocation line title.
        /// </summary>
        /// <param name="allocationLineType">The AllocationLineType.</param>
        /// <returns>The allocation lines.</returns>
        private static string GetAllocationLineTitle(AllocationLineType allocationLineType)
        {
            if (allocationLineType != AllocationLineType.Unknown)
            {
                return allocationLineType.GetPropertyValue<AllocationLineType, DisplayAttribute, string>(o => o.Name!);
            }

            return string.Empty;
        }

        /// <summary>
        /// Converts the search result from the search service into a response object.
        /// </summary>
        /// <param name="searchResult">The search result from the service.</param>
        /// <param name="laCode">(Optional) If not null, the collection of Local Authorities will be filtered to the given code.</param>
        /// <returns>The search response object.</returns>
        private IAllocationApiSearchResponse? GetApiSearchResponse(IAllocationSearchResult searchResult, int? laCode = null)
        {
            if (searchResult == null)
            {
                return null;
            }

            IEnumerable<LocalAuthority>? localAuthorities = searchResult.LaGroups?
                .Select(la => JsonConvert.DeserializeObject<LocalAuthority>(la) !);

            if (localAuthorities != null && laCode.HasValue)
            {
                localAuthorities = localAuthorities.Where(la => la?.LaCode == laCode.Value.ToString());
            }

            var allocations = searchResult.Documents?
                .GroupBy(d => d.Ukprn)?
                .Select(g => g.OrderByDescending(a => a.Version).First())?
                .Where(d => !d.ExcludeFromResults)?
                .Select(a => _mapper.Map<AllocationApiSearchAllocation>(a));

            return new AllocationApiSearchResponse
            {
                Allocations = allocations,
                LocalAuthorities = localAuthorities
            };
        }

        /// <summary>
        /// Gets the allocation funding fundingStream.
        /// </summary>
        /// <param name="fundingStream">The cosmos db allocation object.</param>
        /// <returns>The allocation funding fundingStream.</returns>
        private AllocationFundingStream GetAllocationFundingStream(IFundingStream fundingStream)
        {
            var learnerSupportValueLine = fundingStream.Lines.FirstOrDefault(line =>
                "of which learner support".Equals(line.Name, StringComparison.InvariantCultureIgnoreCase));

            return new AllocationFundingStream
            {
                Title = fundingStream.Name,
                Baseline = (decimal)(fundingStream.BaselineAmount ?? -1),
                Total = (decimal)(fundingStream.TotalAmount ?? -1),
                SplitA = (decimal)(fundingStream.Envelopes?.FirstOrDefault()?.Amount ?? -1),
                SplitB = (decimal)(fundingStream.Envelopes?.LastOrDefault()?.Amount ?? -1),
                LearnerSupportValue = learnerSupportValueLine != null ? (decimal?)learnerSupportValueLine.Amount : null,
                Type = fundingStream.Name.GetEnumFromPropertyValue<AllocationFundingStreamType, DisplayAttribute, string>(e => e.ShortName!),
                AllocationLines = fundingStream.Lines.Select(GetAllocationLine).Where(line => !string.IsNullOrWhiteSpace(line.Title)).ToList()
            };
        }

        /// <summary>
        /// Gets the allocation lines from the mapped line datasource.
        /// </summary>
        /// <param name="line">The allocation line from Cosmos db.</param>
        /// <returns>The allocation lines.</returns>
        private AllocationLine GetAllocationLine(ILine line)
        {
            var allocationLine = new AllocationLine
            {
                Total = (decimal)line.Amount !,
                Type =
                    line.Name.ToLower().GetEnumFromPropertyValue<AllocationLineType, DisplayAttribute, string>(e => e.ShortName!),
            };

            allocationLine.Title = GetAllocationLineTitle(allocationLine.Type);
            return allocationLine;
        }

        /// <summary>
        /// Get datetime of the historical action.
        /// </summary>
        /// <param name="history">The audit history of the allocation.</param>
        /// <returns>The datetime of the history.</returns>
        private DateTime? GetDateTime(IEnumerable<ITypeHistory> history)
        {
            return history?.Select(item => item.ActionDateTimeUtc).FirstOrDefault();
        }

        /// <summary>
        /// Gets the type of allocation back.
        /// </summary>
        /// <param name="typeName">the Type's Name.</param>
        /// <param name="year">the Type's year..</param>
        /// <returns>The allocation type.</returns>
        private FundingStatementType GetType(string typeName, int year)
        {
            switch (typeName)
            {
                case "Adults":
                    return FindCorrectAdultType(year);
                case "Traineeships":
                    return FundingStatementType.Traineeships;
                case "Loans":
                    return FundingStatementType.AdvancedLearnerLoans;
                case "Apprenticeships":
                    return FundingStatementType.ApprenticeshipCarryIn;
                case "TraineeshipProcurement19to24":
                    return FundingStatementType.TraineeshipProcurement19to24;
                case "Apprenticeshipnon-levy":
                    return FundingStatementType.ApprenticeshipNonLevy;
            }

            return FundingStatementType.Other;
        }

        private static FundingStatementType FindCorrectAdultType(int year)
        {
            switch (year)
            {
                case <= 2324:
                    return FundingStatementType.AdultEducationBudget;
                case 2425:
                    return FundingStatementType.ESFAAdultSkillsFund;
                default:
                    return FundingStatementType.DFEAdultSkillsFund;
            }
        }

        #endregion
    }
}