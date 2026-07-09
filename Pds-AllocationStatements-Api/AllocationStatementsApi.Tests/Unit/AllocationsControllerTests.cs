using AllocationStatementsApi.Controllers;
using AllocationStatementsApi.Enums;
using AllocationStatementsApi.Helpers;
using AllocationStatementsApi.Models;
using AllocationStatementsApi.Services.Enums;
using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Implementations.IFileMetadata.Models;
using AllocationStatementsApi.Services.Interfaces;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Pds.Core.Logging;
using System.Linq;
using FileAction = AllocationStatementsApi.Services.Enums.FileAction;

namespace AllocationStatementsApi.Tests.Unit
{
    /// <summary>
    /// The Allocations Controller Unit tests.
    /// </summary>
    [TestClass]
    public class AllocationsControllerTests : BaseDomainUnitLogicTest
    {
        private readonly ILoggerAdapter<AllocationsController> _logger
            = Mock.Of<ILoggerAdapter<AllocationsController>>();

        private readonly Mock<IAllocationRepository> _mockAllocationRepository = new(MockBehavior.Strict);

        private readonly Mock<IAllocationSearchService> _mockAllocationSearchService = new(MockBehavior.Strict);

        private IMapper _mapper = null!;

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            SetMapperHelper();
        }

        #endregion

        #region GetByUkprn

        [TestMethod, TestCategory("Unit")]
        public void GetByUkprn_ReturnsOneAllocationStatement()
        {
            // Arrange
            var allocations = Task.Run(() => new List<IType>
            {
                GetRawAllocationStatement(MainUkprn, FundingStatementType.DFEAdultSkillsFund, 2526)
            });
            const string id = "a47cb3fe-0016-4b57-9fcf-c9313dc97df4";
            _mockAllocationRepository.Setup(o => o.GetAllocationStatementsByUkprn(MainUkprn)).Returns(allocations);
            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            var expected = new AdultFundingStatementsResult
            {
                AllocationStatements = new List<AllocationStatement>
                {
                    GetExpectedDFEAdultSkillsFundStatement(id, MainUkprn)
                }
            };

            // Act
            var result = allocationsController.GetByUkprn(MainUkprn, "principalId");

            // Assert
            result.Result.Should().BeEquivalentTo(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetByUkprn_ReturnsThreeAllocationStatements()
        {
            // Arrange
            var allocations = Task.Run(() => new List<IType>
            {
                GetRawAllocationStatement(MainUkprn, FundingStatementType.AdultEducationBudget),
                GetRawAllocationStatement(MainUkprn, FundingStatementType.ApprenticeshipCarryIn),
                GetRawAllocationStatement(MainUkprn, FundingStatementType.AdvancedLearnerLoans)
            });

            _mockAllocationRepository.Setup(o => o.GetAllocationStatementsByUkprn(MainUkprn)).Returns(allocations);
            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = allocationsController.GetByUkprn(MainUkprn, "principalId");

            var expected = new AdultFundingStatementsResult
            {
                AllocationStatements = new List<AllocationStatement>
                {
                    new AllocationStatement
                    {
                        CreatedAtDate = new DateTime(2019, 3, 11),
                        Id = "a47cb3fe-0016-4b57-9fcf-c9313dc97df4",
                        Period = "1920",
                        TotalValue = 900,
                        Type = FundingStatementType.AdultEducationBudget,
                        Ukprn = MainUkprn,
                        Version = 1,
                        AllocationFundingStreams = new List<AllocationFundingStream>
                        {
                            new AllocationFundingStream
                            {
                                Title = "AEB (cfs)",
                                Baseline = 400,
                                SplitA = 200,
                                SplitB = 250,
                                Total = 450,
                                Type = AllocationFundingStreamType.AdultEducationBudgetContractForService,
                                AllocationLines = new List<AllocationLine>
                                {
                                    new AllocationLine
                                    {
                                        Title = "Adult skills allocation",
                                        Total = 50,
                                        Type = AllocationLineType.AdultSkillsAllocation
                                    },
                                    new AllocationLine
                                    {
                                        Title = "Community learning allocation",
                                        Total = 0,
                                        Type = AllocationLineType.CommunityLearningAllocation2
                                    },
                                    new AllocationLine
                                    {
                                        Title = "19 to 24 traineeships allocation",
                                        Total = 0,
                                        Type = AllocationLineType.NineteenToTwentyFourTraineeshipsAllocation
                                    },
                                    new AllocationLine
                                    {
                                        Title = "Continuing learners in a devolved area",
                                        Total = 200,
                                        Type = AllocationLineType.ContinuingLearnersInADevolvedArea
                                    },
                                    new AllocationLine
                                    {
                                        Title = "Continuing learners outside a devolved area",
                                        Total = 0,
                                        Type = AllocationLineType.ContinuingLearnersOutsideADevolvedArea
                                    }
                                }
                            },

                            new AllocationFundingStream
                            {
                                Title = "AEB (grant)",
                                Baseline = 400,
                                SplitA = 200,
                                SplitB = 250,
                                Total = 450,
                                Type = AllocationFundingStreamType.AdultEducationBudgetGrant,
                                AllocationLines = new List<AllocationLine>
                                {
                                    new AllocationLine
                                    {
                                        Title = "Adult skills allocation",
                                        Total = 50,
                                        Type = AllocationLineType.AdultSkillsAllocation
                                    },
                                    new AllocationLine
                                    {
                                        Title = "Community learning allocation",
                                        Total = 0,
                                        Type = AllocationLineType.CommunityLearningAllocation2
                                    },
                                    new AllocationLine
                                    {
                                        Title = "19 to 24 traineeships allocation",
                                        Total = 0,
                                        Type = AllocationLineType.NineteenToTwentyFourTraineeshipsAllocation
                                    },
                                    new AllocationLine
                                    {
                                        Title = "Continuing learners in a devolved area",
                                        Total = 0,
                                        Type = AllocationLineType.ContinuingLearnersInADevolvedArea
                                    },
                                    new AllocationLine
                                    {
                                        Title = "Continuing learners outside a devolved area",
                                        Total = 200,
                                        Type = AllocationLineType.ContinuingLearnersOutsideADevolvedArea
                                    }
                                }
                            }
                        },
                        HasBeenRead = false
                    },
                    new AllocationStatement
                    {
                        CreatedAtDate = new DateTime(2019, 3, 11),
                        Id = "a47cb3fe-0016-4b57-9fcf-c9313dc97df4",
                        Period = "1920",
                        TotalValue = 450,
                        Type = FundingStatementType.ApprenticeshipCarryIn,
                        Ukprn = MainUkprn,
                        Version = 1,
                        AllocationFundingStreams = new List<AllocationFundingStream>
                        {
                            new AllocationFundingStream
                            {
                                Title = "Apprenticeship carry-in",
                                Baseline = 400,
                                SplitA = 200,
                                SplitB = 250,
                                Total = 450,
                                Type = AllocationFundingStreamType.ApprenticeshipCarryIn,
                                AllocationLines = new List<AllocationLine>()
                                {
                                    new AllocationLine
                                    {
                                        Title = "16 to 18 apprentices that started before 1 May 2017",
                                        Total = 200M,
                                        Type = AllocationLineType.SixteenToEightteenCarryInApprenticesPreMay
                                    },
                                    new AllocationLine
                                    {
                                        Title = "19+ apprentices that started before 1 May 2017",
                                        Total = 250M,
                                        Type = AllocationLineType.NineteenPlusCarryInApprenticesPreMay
                                    }
                                }
                            }
                        },
                        HasBeenRead = false
                    },
                    new AllocationStatement
                    {
                        CreatedAtDate = new DateTime(2019, 3, 11),
                        Id = "a47cb3fe-0016-4b57-9fcf-c9313dc97df4",
                        Period = "1920",
                        TotalValue = 900,
                        Type = FundingStatementType.AdvancedLearnerLoans,
                        Ukprn = MainUkprn,
                        Version = 1,
                        AllocationFundingStreams = new List<AllocationFundingStream>
                        {
                            new AllocationFundingStream
                            {
                                Title = "Loan facility",
                                Baseline = 400,
                                SplitA = 200,
                                SplitB = 250,
                                Total = 450,
                                Type = AllocationFundingStreamType.LoanFacillity,
                                AllocationLines = new List<AllocationLine>()
                            },
                            new AllocationFundingStream
                            {
                                Title = "Loan bursary",
                                Baseline = 400,
                                SplitA = 200,
                                SplitB = 250,
                                Total = 450,
                                Type = AllocationFundingStreamType.LoanBursary,
                                AllocationLines = new List<AllocationLine>()
                            }
                        },
                        HasBeenRead = false
                    },
                }
            };

            // Assert
            result.Result.Should().BeEquivalentTo(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetByUkprn_WhereNoResultsReturned()
        {
            // Arrange
            var allocations = Task.Run(() => new List<IType>());
            _mockAllocationRepository.Setup(o => o.GetAllocationStatementsByUkprn(MainUkprn)).Returns(allocations);
            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = allocationsController.GetByUkprn(MainUkprn, "principalId");

            var expected = new AdultFundingStatementsResult
            {
                AllocationStatements = new List<AllocationStatement>()
            };

            // Assert
            result.Result.Should().BeEquivalentTo(expected);
        }

        #endregion


        [TestMethod, TestCategory("Unit")]
        public void GetById_ReturnsAnAllocation()
        {
            // Arrange
            var allocations = Task.Run(() => new List<IType>
            {
                GetRawAllocationStatement(MainUkprn, FundingStatementType.DFEAdultSkillsFund, 2526)
            });
            const string id = "a47cb3fe-0016-4b57-9fcf-c9313dc97df4";
            _mockAllocationRepository.Setup(o => o.GetAllocationStatementById(id, MainUkprn)).Returns(allocations);
            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            var expected = new AdultFundingStatementResult
            {
                AllocationStatement = GetExpectedDFEAdultSkillsFundStatement(id, MainUkprn)
            };

            // Act
            var result = allocationsController.GetById(id, MainUkprn);

            // Assert
            result.Result.Should().BeEquivalentTo(expected);
        }


        #region GetExistsByUkprn

        [TestMethod, TestCategory("Unit")]
        public void GetExistsByUkprn_WhereAllocationsExistAndOneAllocationNotRead()
        {
            // Arrange
            const string principalId = "Principal";

            var readAllocation = GetRawAllocationStatement(MainUkprn, FundingStatementType.AdultEducationBudget);
            var readHistory = new TypeHistory()
            {
                Action = FileAction.ViewedInDetail,
                User = new TypeHistoryUser() { Principle = principalId },
                ActionDateTimeUtc = new DateTime(2019, 3, 13)
            };

            readAllocation.History = readAllocation.History.Append(readHistory);

            var allocations = new List<IType>
            {
                readAllocation,
                GetRawAllocationStatement(MainUkprn, FundingStatementType.ApprenticeshipNonLevy),
            };

            _mockAllocationRepository.Setup(o => o.GetAllocationStatementsByUkprn(MainUkprn)).Returns(Task.Run(() => allocations));

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            var expected = new AdultFundingStatementExistsResult
            {
                AllocationExists = true, 
                UnreadNewAllocations = 1,
                UnreadUpdatedAllocations = 0
            };

            // Act
            var result = allocationsController.GetExistsByUkprn(MainUkprn, principalId);

            // Assert
            result.Result.Should().BeEquivalentTo(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetExistsByUkprn_WhereAllocationsExistAndFourAllocationsNotRead()
        {
            // Arrange
            const string principalId = "Principal";

            var readAEBAllocationVersion1 = GetRawAllocationStatement(MainUkprn, FundingStatementType.AdultEducationBudget);
            var readHistory = new TypeHistory()
            {
                Action = FileAction.ViewedInDetail,
                User = new TypeHistoryUser() { Principle = principalId },
                ActionDateTimeUtc = new DateTime(2019, 3, 13)
            };
            readAEBAllocationVersion1.History = readAEBAllocationVersion1.History.Append(readHistory);

            var readApprenticeshipVersion1 = GetRawAllocationStatement(MainUkprn, FundingStatementType.ApprenticeshipNonLevy);
            readApprenticeshipVersion1.History = readApprenticeshipVersion1.History.Append(readHistory);

            var readApprenticeshipVersion2 = GetRawAllocationStatement(MainUkprn, FundingStatementType.ApprenticeshipNonLevy, 1920, 2);
            readApprenticeshipVersion2.History = readApprenticeshipVersion2.History.Append(readHistory);
            
            var readTraineeshipAllocation1 = GetRawAllocationStatement(MainUkprn, FundingStatementType.Traineeships);
            readTraineeshipAllocation1.History = readTraineeshipAllocation1.History.Append(readHistory);

            var allocations = new List<IType>
            {
                readAEBAllocationVersion1,
                readApprenticeshipVersion1,
                readApprenticeshipVersion2,
                GetRawAllocationStatement(MainUkprn, FundingStatementType.ApprenticeshipNonLevy, 1920, 3),
                readTraineeshipAllocation1,
                GetRawAllocationStatement(MainUkprn, FundingStatementType.Traineeships, 1920, 2),
                GetRawAllocationStatement(MainUkprn, FundingStatementType.ApprenticeshipCarryIn),
                GetRawAllocationStatement(MainUkprn, FundingStatementType.AdvancedLearnerLoans),
            };

            _mockAllocationRepository.Setup(o => o.GetAllocationStatementsByUkprn(MainUkprn)).Returns(Task.Run(() => allocations));

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            var expected = new AdultFundingStatementExistsResult
            {
                AllocationExists = true,
                UnreadNewAllocations = 2,
                UnreadUpdatedAllocations = 2
            };

            // Act
            var result = allocationsController.GetExistsByUkprn(MainUkprn, principalId);

            // Assert
            result.Result.Should().BeEquivalentTo(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetExistsByUkprn_WhereNoAllocationsExistAndZeroAllocationsNotRead()
        {
            // Arrange
            const string principalId = "Principal";

            _mockAllocationRepository.Setup(o => o.GetAllocationStatementsByUkprn(MainUkprn)).Returns(Task.Run(() => new List<IType>()));

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            var expected = new AdultFundingStatementExistsResult
            {
                AllocationExists = false,
                UnreadNewAllocations = 0,
                UnreadUpdatedAllocations = 0
            };

            // Act
            var result = allocationsController.GetExistsByUkprn(MainUkprn, principalId);

            // Assert
            result.Result.Should().BeEquivalentTo(expected);
        }

        #endregion

        [TestMethod, TestCategory("Unit")]
        public void SetAllocationAsReadById_ReturnsTrue()
        {
            // Arrange
            var input = new AllocationViewedInfo
            {
                id = "a47cb3fe-0016-4b57-9fcf-c9313dc97df4",
                ukprn = MainUkprn,
                principleId = "principleId"
            };

            _mockAllocationRepository.Setup(o => o.SetAllocationAsReadById(input.id, input.ukprn, input.principleId)).Returns(Task.Run(() => true));
            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = allocationsController.SetAllocationAsReadById(input);

            // Assert
            result.Result.Should().Be(true);
        }


        #region SearchProvider Tests

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_PassesThroughParameters()
        {
            // Arrange
            var inputs = new
            {
                AllocationType = FundingStatementType.AdvancedLearnerLoans,
                SearchTerm = "St. Mary's",
                CutOff = new DateTime(2002, 6, 3, 4, 25, 57),
                Year = 200102
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(inputs.AllocationType.ToString(), inputs.SearchTerm, inputs.CutOff, inputs.Year))
                .ReturnsAsync(It.IsAny<IAllocationSearchResult>())
                .Verifiable();

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchProvider(
                inputs.AllocationType, inputs.SearchTerm, inputs.CutOff, inputs.Year);

            // Assert
            _mockAllocationSearchService.Verify();
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_HandlesNullResult()
        {
            // Arrange
            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync((AzureAllocationSearchResult)null!);

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_HandlesNullDocuments()
        {
            // Arrange
            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult());

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Allocations);
            Assert.IsNull(result.LocalAuthorities);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.78M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 1,
                        Year = 201819
                    }
                },
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_OneProviderWithOneVersion_Excluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_OneProviderWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.79M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.80M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 3,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.80M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 3,
                        Year = 201819
                    }
                },
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_OneProviderWithMultipleVersions_NonLatestExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.79M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.80M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 3,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.80M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 3,
                        Year = 201819
                    }
                },
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_OneProviderWithMultipleVersions_LatestExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.79M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.80M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 3,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 0M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 4,
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.03M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 1,
                    Year = 201819
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 99999.01M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 1,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 99999.02M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 1,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 3",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 99999.03M,
                        Town = "Fake Town",
                        Ukprn = "10000003",
                        Version = 1,
                        Year = 201819
                    }
                },
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_MultipleProvidersEachWithOneVersion_SomeExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.03M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 99999.02M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 1,
                        Year = 201819
                    }
                },
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_MultipleProvidersEachWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.03M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.04M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.05M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 9999999.06M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.07M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 3,
                    Year = 201819
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.04M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.05M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 3",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.07M,
                        Town = "Fake Town",
                        Ukprn = "10000003",
                        Version = 3,
                        Year = 201819
                    }
                },
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_MultipleProvidersEachWithMultipleVersions_SomeExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.03M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.04M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.05M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 2,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 9999999.06M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.07M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 3,
                    Year = 201819
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.04M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 3",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.07M,
                        Town = "Fake Town",
                        Ukprn = "10000003",
                        Version = 3,
                        Year = 201819
                    }
                },
                LocalAuthorities = null
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region SearchLocalAuthority Tests

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_PassesThroughParameters()
        {
            // Arrange
            var inputs = new
            {
                AllocationType = FundingStatementType.ApprenticeshipNonLevy,
                SearchTerm = "Nottingham",
                CutOff = new DateTime(2002, 6, 3, 4, 25, 57),
                Year = 200102
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(inputs.AllocationType.ToString(), inputs.SearchTerm, inputs.CutOff, inputs.Year, null))
                .ReturnsAsync(It.IsAny<IAllocationSearchResult>())
                .Verifiable();

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchLocalAuthority(
                inputs.AllocationType, inputs.SearchTerm, inputs.CutOff, inputs.Year);

            // Assert
            _mockAllocationSearchService.Verify();
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_HandlesNullResult()
        {
            // Arrange
            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync((AzureAllocationSearchResult)null!);

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_HandlesNullDocuments()
        {
            // Arrange
            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult());

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Allocations);
            Assert.IsNull(result.LocalAuthorities);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.78M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 1,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_OneProviderWithOneVersion_Excluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_OneProviderWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_OneProviderWithMultipleVersions_NonLatestExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_OneProviderWithMultipleVersions_LatestExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.78M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 1,
                        Year = 201819
                    },
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_MultipleProvidersEachWithOneVersion_SomeExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.78M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 1,
                        Year = 201819
                    },
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_MultipleProvidersEachWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.03M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.04M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.05M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 9999999.06M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority 2\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority 2",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.07M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 3,
                    Year = 201819
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.04M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.05M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "1234",
                        LaName = "Fake Local Authority 2",
                        Name = "Fake School 3",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.07M,
                        Town = "Fake Town",
                        Ukprn = "10000003",
                        Version = 3,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" },
                    new LocalAuthority { LaCode = "1234", LaName = "Fake Local Authority 2" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_MultipleProvidersEachWithMultipleVersions_SomeExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.03M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.04M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.05M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 9999999.06M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority 2\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority 2",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.07M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
                    Version = 3,
                    Year = 201819,
                    ExcludeFromResults = true
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.05M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" },
                    new LocalAuthority { LaCode = "1234", LaName = "Fake Local Authority 2" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), null))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region SearchLocalAuthorityByCode Tests

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_HandlesNullResult()
        {
            // Arrange
            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<Func<IAllocationSearchDocument, bool>>()))
                .ReturnsAsync((AzureAllocationSearchResult)null!);

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_HandlesNullDocuments()
        {
            // Arrange
            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<Func<IAllocationSearchDocument, bool>>()))
                .ReturnsAsync(new AzureAllocationSearchResult());

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Allocations);
            Assert.IsNull(result.LocalAuthorities);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_PassesThroughParametersAndFilter()
        {
            // Arrange
            var inputs = new
            {
                AllocationType = FundingStatementType.ESFAAdultSkillsFund,
                LaCode = 12345,
                CutOff = new DateTime(2002, 6, 3, 4, 25, 57),
                Year = 200102
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    inputs.AllocationType.ToString(),
                    inputs.LaCode.ToString(),
                    inputs.CutOff,
                    inputs.Year,
                    It.Is<Func<IAllocationSearchDocument, bool>>(d => d != null)))
                .ReturnsAsync(It.IsAny<IAllocationSearchResult>())
                .Verifiable();

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = await allocationsController.SearchLocalAuthorityByCode(
                inputs.AllocationType, inputs.LaCode, inputs.CutOff, inputs.Year);

            // Assert
            _mockAllocationSearchService.Verify();
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.78M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 1,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_OneProviderWithOneVersion_Excluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_OneProviderWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_OneProviderWithMultipleVersions_NonLatestExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456M,
                        Town = "Fake Town",
                        Ukprn = "12345678",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_OneProviderWithMultipleVersions_LatestExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 2,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "12345678",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456.78M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 1,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_MultipleProvidersEachWithOneVersion_SomeExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 123456M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_MultipleProvidersEachWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.04M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.05M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 2,
                    Year = 201819
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 1",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.04M,
                        Town = "Fake Town",
                        Ukprn = "10000001",
                        Version = 2,
                        Year = 201819
                    },
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.05M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthorityByCode_MultipleProvidersEachWithMultipleVersions_SomeExcluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.01M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 99999.02M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 1,
                    Year = 201819
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 1",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.04M,
                    Town = "Fake Town",
                    Ukprn = "10000001",
                    Version = 2,
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":123, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "123",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 2",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 88888.05M,
                    Town = "Fake Town",
                    Ukprn = "10000002",
                    Version = 2,
                    Year = 201819
                },
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>
                {
                    new AllocationApiSearchAllocation
                    {
                        EstablishmentNumber = "123456",
                        LaCode = "123",
                        LaName = "Fake Local Authority",
                        Name = "Fake School 2",
                        Postcode = "AA11 1AA",
                        ProviderSubType = "Fake Provider Sub Type",
                        ProviderType = "Fake Provider Type",
                        PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                        Streams = new List<FundingStream> { JsonConvert.DeserializeObject<FundingStream>("{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}") ! },
                        TotalAmount = 88888.05M,
                        Town = "Fake Town",
                        Ukprn = "10000002",
                        Version = 2,
                        Year = 201819
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAllocationSearchService
                .Setup(s => s.SearchLocalAuthority(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.Is<Func<IAllocationSearchDocument, bool>>(f => f != null)))
                .ReturnsAsync(new AzureAllocationSearchResult
                {
                    Documents = documents,
                    LaGroups = documents.Select(d => d.LaGroup).Distinct().ToList()
                });

            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var actualResult = await allocationsController.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        [TestMethod, TestCategory("Unit")]
        public void GetCountByCreatedAtDateRange_ReturnsCountOfStatements()
        {
            // Arrange
            var startDate = new DateTime(2020, 3, 1);
            var endDate = new DateTime(2020, 3, 31);

            var rawAllocation1 = GetRawAllocationStatement(MainUkprn, FundingStatementType.AdultEducationBudget);
            rawAllocation1.History.First().ActionDateTimeUtc = startDate;

            var rawAllocation2 = GetRawAllocationStatement(MainUkprn, FundingStatementType.ApprenticeshipCarryIn);
            rawAllocation2.History.First().ActionDateTimeUtc = startDate.AddDays(2);

            var rawAllocation3 = GetRawAllocationStatement(MainUkprn, FundingStatementType.AdvancedLearnerLoans);
            rawAllocation3.History.First().ActionDateTimeUtc = endDate.AddDays(-10);

            var allocationsCount = Task.Run(() => 3);

            _mockAllocationRepository.Setup(o => o.GetAllocationStatementCountByCreatedAtDateRange(startDate.ToString("s"), endDate.ToString("s"))).Returns(allocationsCount);
            var allocationsController = new AllocationsController(_mockAllocationRepository.Object, _mockAllocationSearchService.Object, _logger, _mapper);

            // Act
            var result = allocationsController.GetCountByCreatedAtDateRange(startDate.ToString("s"), endDate.ToString("s"));

            var expected = 3;

            // Assert
            result.Result.Should().Be(expected);
        }

        /// <summary>
        /// Set the mapper config.
        /// </summary>
        private void SetMapperHelper()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            _mapper = mapperConfig.CreateMapper();
        }
    }
}