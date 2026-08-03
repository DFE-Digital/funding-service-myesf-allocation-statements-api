using AllocationStatementsApi.Controllers;
using AllocationStatementsApi.Enums;
using AllocationStatementsApi.Services.Implementations.IAllocationSearchService;
using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Implementations.IFileMetadata.Models;
using AllocationStatementsApi.Services.Interfaces;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;
using Azure.Search.Documents;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Pds.Core.Logging;

namespace AllocationStatementsApi.Tests.Integration
{
    [TestClass]
    public class AllocationsControllerTests
    {
        private const string LaIndexName = "LA";
        private const string ProviderIndexName = "PROVIDER";

        private readonly ILoggerAdapter<AllocationsController> _logger
            = Mock.Of<ILoggerAdapter<AllocationsController>>();

        private readonly Mock<IAzureSearchIndexClient> _mockAzureSearchIndexClient
            = new(MockBehavior.Strict);

        private readonly Mock<IAzureSearchIndexClientManager> _mockAzureSearchIndexClientManager
            = new(MockBehavior.Strict);

        #region SearchProviders Tests

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviders_Empty()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>();
            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = null
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchProviders_OneProviderWithOneVersion()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchProviders_OneProviderWithOneVersion_Excluded()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviders_OneProviderWithMultipleVersions()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchProviders_OneProviderWithMultipleVersions_Excluded()
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
                    Year = 201819,
                    ExcludeFromResults = true
                }
            };

            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = null
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchProviders_MultipleProvidersEachWithOneVersion()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchProviders_MultipleProvidersEachWithOneVersion_Excluded()
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
                    }
                },
                LocalAuthorities = null
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviders_MultipleProvidersEachWithMultipleVersions()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchProviders_MultipleProvidersEachWithMultipleVersions_Excluded()
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
                LocalAuthorities = null
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(ProviderIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchProvider(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region SearchLocalAuthority Tests

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchLocalAuthority_Empty()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>();
            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = new List<LocalAuthority>()
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchLocalAuthority_OneProviderWithOneVersion_Excluded()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>
            {
                new AzureAllocationSearchDocument()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchLocalAuthority_OneProviderWithMultipleVersions_Excluded()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object,
                new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null),
                _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchLocalAuthority_MultipleProvidersEachWithOneVersion_Excluded()
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
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchLocalAuthority_MultipleProvidersEachWithMultipleVersions_Excluded()
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
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" },
                    new LocalAuthority { LaCode = "1234", LaName = "Fake Local Authority 2" }
                }
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthority(
                It.IsAny<FundingStatementType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region SearchLocalAuthorityByCode Tests

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchLocalAuthorityByCode_Empty()
        {
            // Arrange
            var documents = new List<AzureAllocationSearchDocument>();
            var expectedResult = new AllocationApiSearchResponse
            {
                Allocations = new List<AllocationApiSearchAllocation>(),
                LocalAuthorities = new List<LocalAuthority>()
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchLocalAuthorityByCode_OneProviderWithMultipleVersions_Excluded()
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
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
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchLocalAuthorityByCode_MultipleProvidersEachWithOneVersion_Excluded()
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
                    Year = 201819,
                    ExcludeFromResults = true
                },
                new AzureAllocationSearchDocument
                {
                    EstablishmentNumber = "123456",
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority",
                    Name = "Fake School 3",
                    Postcode = "AA11 1AA",
                    ProviderSubType = "Fake Provider Sub Type",
                    ProviderType = "Fake Provider Type",
                    PublishedDateTime = new DateTime(2019, 01, 01, 6, 43, 20),
                    Streams = new[] { "{\"name\":\"PE and Sport\",\"baselineAmount\":null,\"envelopes\":[{\"name\":\"October 2018 (1X - CalendarMonth AY1819)\",\"amount\":10978},{\"name\":\"April 2019 (1X - CalendarMonth AY1819)\",\"amount\":7842}],\"lines\":[{\"name\":\"Maintained Schools\",\"amount\":18820}],\"totalAmount\":18820}" },
                    TotalAmount = 123456.78M,
                    Town = "Fake Town",
                    Ukprn = "10000003",
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

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
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
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority 2\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority 2",
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
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority 2\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority 2",
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
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                  .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SearchLocalAuthorityByCode_MultipleProvidersEachWithMultipleVersions_Excluded()
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
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority 2\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority 2",
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
                    LaGroup = "{\"LaCode\":1234, \"LaName\":\"Fake Local Authority 2\"}",
                    LaCode = "1234",
                    LaName = "Fake Local Authority 2",
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
                    Year = 201819,
                    ExcludeFromResults = true
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
                    }
                },
                LocalAuthorities = new List<LocalAuthority>
                {
                    new LocalAuthority { LaCode = "123", LaName = "Fake Local Authority" }
                }
            };

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                 .ReturnsAsync(documents);

            _mockAzureSearchIndexClient
                .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetDocumentAndLASearchResult(documents));

            _mockAzureSearchIndexClientManager
                .Setup(m => m.GetSearchIndexClient(LaIndexName))
                .Returns(_mockAzureSearchIndexClient.Object);

            var controller = new AllocationsController(
                new Mock<IAllocationRepository>().Object, new AzureAllocationSearchService(_mockAzureSearchIndexClientManager.Object, ProviderIndexName, LaIndexName, null), _logger);

            // Act
            var actualResult = await controller.SearchLocalAuthorityByCode(
                It.IsAny<FundingStatementType>(), 123, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region Private Helpers

        private object[] GetDocumentAndLASearchResult(List<AzureAllocationSearchDocument> documents)
        {
            var laGroups = new List<string>();
            var docAndLaResults = new object[] { documents, new List<string>() };

            foreach (var document in documents)
            {
                laGroups.Add(document.LaGroup);
            }

            docAndLaResults[1] = laGroups.Distinct().ToList();

            return docAndLaResults;
        }

        #endregion
    }
}