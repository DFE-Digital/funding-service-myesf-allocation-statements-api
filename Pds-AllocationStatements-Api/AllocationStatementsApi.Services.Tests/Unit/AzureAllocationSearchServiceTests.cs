using AllocationStatementsApi.Services.Implementations.IAllocationSearchService;
using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllocationStatementsApi.Services.Tests.Unit
{
    [TestClass]
    public class AzureAllocationSearchServiceTests
    {
        #region Test Constants

        private const string ProviderIndexName = "PROVIDER_INDEX";
        private const string LaIndexName = "LA_INDEX";
        private const int LaFirstSearchSize = 10;
        private const int AzureSearchMaxResults = 1000;

        private static readonly MockDocumentConfiguration[] DefaultMockDocumentConfiguration
            = new[] { new MockDocumentConfiguration(0, 999, 1) };

        #endregion


        #region SearchLocalAuthority Tests

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_UsesLaIndexClient()
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(new[] { new MockDocumentConfiguration(0, LaFirstSearchSize, 1) }, 1);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), "search", It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            clientManager.Verify(m => m.GetSearchIndexClient(LaIndexName), Times.Once);
            clientManager.Verify(m => m.GetSearchIndexClient(ProviderIndexName), Times.Never);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(null, 201819, 1000, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(null, null, 1000, "PublishedDate lt {0}")]
        [DataRow(-1, 201819, 1000, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(-1, null, 1000, "PublishedDate lt {0}")]
        [DataRow(0, 201819, 0, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(0, null, 0, "PublishedDate lt {0}")]
        [DataRow(1, 200102, 1, "PublishedDate lt {0} and year eq 200102")]
        [DataRow(1, null, 1, "PublishedDate lt {0}")]
        [DataRow(10, 201112, 10, "PublishedDate lt {0} and year eq 201112")]
        [DataRow(10, null, 10, "PublishedDate lt {0}")]
        [DataRow(10, 0, 10, "PublishedDate lt {0}")]
        [DataRow(10, -1, 10, "PublishedDate lt {0}")]
        [DataRow(999, 201819, 999, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(999, null, 999, "PublishedDate lt {0}")]
        [DataRow(1000, 201819, 1000, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(1000, null, 1000, "PublishedDate lt {0}")]
        [DataRow(1001, 201819, 1000, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(1001, null, 1000, "PublishedDate lt {0}")]
        [DataRow(9999, 201819, 1000, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(9999, null, 1000, "PublishedDate lt {0}")]
        public async Task SearchLocalAuthority_CheckParameters(
            int? laFirstSearchSize, int? academicYear, int expectedSearchSize, string expectedFilter)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[]
                {
                    new MockDocumentConfiguration(0, expectedSearchSize, expectedSearchSize >= 1 ? 1 : 0),
                    new MockDocumentConfiguration(expectedSearchSize, AzureSearchMaxResults, 0)
                },
                1);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, laFirstSearchSize);
            var beforeDateTime = new DateTime(2002, 6, 24, 3, 37, 49);
            var beforeDateTimeOffset = new DateTimeOffset(beforeDateTime).ToUniversalTime().ToString("O");

            // Act
            var result = await searchService.SearchLocalAuthority(It.IsAny<string>(), "search", beforeDateTime, academicYear);

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == expectedSearchSize
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.QueryType == SearchQueryType.Full
                        && op.Facets.Count == 1
                        && op.Facets[0] == "LaGroup,count:1000000"
                        && op.Filter == string.Format(expectedFilter, beforeDateTimeOffset))),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == expectedSearchSize
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.QueryType == SearchQueryType.Full
                        && op.Facets.Count == 1
                        && op.Facets[0] == "LaGroup,count:1000000"
                        && op.Filter == string.Format(expectedFilter, beforeDateTimeOffset))),
                Times.Never);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(null, "/.*/")]
        [DataRow("", "/.*/")]
        [DataRow(" ", "/.*/")]
        [DataRow("_", "/.*/")]
        [DataRow("-", "/.*/")]
        [DataRow("-_\u2013 ~#@'!£$%^&*()[]{}/\\`¬¦|?><,.:; ", "/.*/")]
        [DataRow("search", "/.*search.*/")]
        [DataRow("$earch", "/.*earch.*/")]
        [DataRow("test-search", "/.*test_search.*/")]
        [DataRow("test $-search_", "/.*test_search.*/")]
        [DataRow(" Test $-search _", "/.*Test_search.*/")]
        public async Task SearchLocalAuthority_CheckSearchText(string searchText, string expectedSearchText)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(new[] { new MockDocumentConfiguration(0, LaFirstSearchSize, 1) }, 1);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchLocalAuthority(It.IsAny<string>(), searchText, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    expectedSearchText, It.IsAny<string>(), It.IsAny<SearchOptions>()),
                Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(null)]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(999)]
        [DataRow(1000)]
        [DataRow(1001)]
        [DataRow(9999)]
        public async Task SearchLocalAuthority_RequestMaxResultsWhenUsingCustomFilter(int? laFirstSearchSize)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(new[] { new MockDocumentConfiguration(0, AzureSearchMaxResults, 1) }, 1);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, laFirstSearchSize);

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), new Func<IAllocationSearchDocument, bool>(_ => It.IsAny<bool>()));

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(), It.IsAny<string>(), It.Is<SearchOptions>(op =>
                            op.Size == AzureSearchMaxResults
                            && (!op.Skip.HasValue || op.Skip.Value == 0))),
                Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        [DataRow(50)]
        public async Task SearchLocalAuthority_OnlyGetsFirstPageWhenMultipleLaGroupsAndNoFilter(int laGroupCount)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[] { new MockDocumentConfiguration(0, LaFirstSearchSize, LaFirstSearchSize) },
                laGroupCount);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == LaFirstSearchSize
                        && (!op.Skip.HasValue || op.Skip.Value == 0))),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op => op.Skip > 0)),
                Times.Never);

            Assert.AreEqual(LaFirstSearchSize, result.Documents.Count());
            Assert.AreEqual(laGroupCount, result.LaGroups.Count());
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        [DataRow(50)]
        public async Task SearchLocalAuthority_GetAllPagesWhenUsingCustomFilter(int laGroupCount)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[]
                {
                    new MockDocumentConfiguration(0, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults * 2, AzureSearchMaxResults, AzureSearchMaxResults - 1)
                }, laGroupCount);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), new Func<IAllocationSearchDocument, bool>(_ => true));

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    "LaGroup",
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.Facets.First() == "LaGroup,count:1000000")),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults * 2
                        && op.Facets.Count == 0)),
                Times.Once);

            Assert.AreEqual(3 * AzureSearchMaxResults - 1, result.Documents.Count());
            Assert.AreEqual(laGroupCount, result.LaGroups.Count());
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchLocalAuthority_GetAllPagesWhenOnlyOneLaGroup()
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[]
                {
                    new MockDocumentConfiguration(0, LaFirstSearchSize, LaFirstSearchSize),
                    new MockDocumentConfiguration(LaFirstSearchSize, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(LaFirstSearchSize + AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults - 1),
                }, 1);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    "LaGroup",
                    It.Is<SearchOptions>(o =>
                        o.Size == LaFirstSearchSize
                        && (!o.Skip.HasValue || o.Skip.Value == 0)
                        && o.Facets.First() == "LaGroup,count:1000000")),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == LaFirstSearchSize
                        && op.Facets.Count == 0)),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == LaFirstSearchSize + AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            Assert.AreEqual(2 * AzureSearchMaxResults + LaFirstSearchSize - 1, result.Documents.Count());
            Assert.AreEqual(1, result.LaGroups.Count());
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        [DataRow(50)]
        public async Task SearchLocalAuthority_CustomFilterIsAppliedToResults_AlwaysFalse(int laGroupCount)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[]
                {
                new MockDocumentConfiguration(0, AzureSearchMaxResults, AzureSearchMaxResults),
                new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults),
                new MockDocumentConfiguration(AzureSearchMaxResults * 2, AzureSearchMaxResults, AzureSearchMaxResults - 1),
                }, laGroupCount);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                new Func<IAllocationSearchDocument, bool>(_ => false));

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    "LaGroup",
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.Facets.First() == "LaGroup,count:1000000")),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults * 2
                        && op.Facets.Count == 0)),
                Times.Once);

            Assert.AreEqual(0, result.Documents.Count());
            Assert.AreEqual(laGroupCount, result.LaGroups.Count());
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        [DataRow(50)]
        public async Task SearchLocalAuthority_CustomFilterIsAppliedToResults_UkprnStartsWith1(int laGroupCount)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[]
                {
                    new MockDocumentConfiguration(0, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults * 2, AzureSearchMaxResults, AzureSearchMaxResults - 1),
                }, laGroupCount);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);
            var filterExpression = new Func<IAllocationSearchDocument, bool>(d => d.Ukprn.StartsWith("1"));

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), filterExpression);

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    "LaGroup",
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.Facets.First() == "LaGroup,count:1000000")),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults * 2
                        && op.Facets.Count == 0)),
                Times.Once);

            Assert.IsTrue(result.Documents.Any());
            Assert.IsTrue(result.Documents.All(filterExpression));
            Assert.AreEqual(laGroupCount, result.LaGroups.Count());
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        [DataRow(50)]
        public async Task SearchLocalAuthority_CustomFilterIsAppliedToResults_UkprnIsOdd(int laGroupCount)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[]
                {
                    new MockDocumentConfiguration(0, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults * 2, AzureSearchMaxResults, AzureSearchMaxResults - 1),
                }, laGroupCount);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);
            var filterExpression = new Func<IAllocationSearchDocument, bool>(d => int.Parse(d.Ukprn) % 2 == 1);

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), filterExpression);

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    "LaGroup",
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.Facets.First() == "LaGroup,count:1000000")),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults * 2
                        && op.Facets.Count == 0)),
                Times.Once);

            Assert.IsTrue(result.Documents.Any());
            Assert.IsTrue(result.Documents.All(filterExpression));
            Assert.AreEqual(laGroupCount, result.LaGroups.Count());
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        [DataRow(50)]
        public async Task SearchLocalAuthority_CustomFilterIsAppliedToResults_UkprnContains37(int laGroupCount)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, null);
            var mockLaIndexClient = GetMockSearchClient(
                new[]
                {
                    new MockDocumentConfiguration(0, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults * 2, AzureSearchMaxResults, AzureSearchMaxResults - 1),
                }, laGroupCount);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);
            var filterExpression = new Func<IAllocationSearchDocument, bool>(d => d.Ukprn.Contains("37"));

            // Act
            var result = await searchService.SearchLocalAuthority(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), filterExpression);

            // Assert
            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    "LaGroup",
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.Facets.First() == "LaGroup,count:1000000")),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            mockLaIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults * 2
                        && op.Facets.Count == 0)),
                Times.Once);

            Assert.IsTrue(result.Documents.Any());
            Assert.IsTrue(result.Documents.All(filterExpression));
            Assert.AreEqual(laGroupCount, result.LaGroups.Count());
        }

        #endregion


        #region SearchProvider Tests

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_UsesProviderIndexClient()
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(new[] { new MockDocumentConfiguration(0, AzureSearchMaxResults, 1) }, null);
            var mockLaIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, 1);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchProvider(
                It.IsAny<string>(), "search", It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            clientManager.Verify(m => m.GetSearchIndexClient(LaIndexName), Times.Never);
            clientManager.Verify(m => m.GetSearchIndexClient(ProviderIndexName), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(null, 201819, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(null, null, "PublishedDate lt {0}")]
        [DataRow(-1, 201819, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(-1, null, "PublishedDate lt {0}")]
        [DataRow(0, 201819, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(0, null, "PublishedDate lt {0}")]
        [DataRow(1, 200102, "PublishedDate lt {0} and year eq 200102")]
        [DataRow(1, null, "PublishedDate lt {0}")]
        [DataRow(10, 201112, "PublishedDate lt {0} and year eq 201112")]
        [DataRow(10, null, "PublishedDate lt {0}")]
        [DataRow(10, 0, "PublishedDate lt {0}")]
        [DataRow(10, -1, "PublishedDate lt {0}")]
        [DataRow(999, 201819, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(999, null, "PublishedDate lt {0}")]
        [DataRow(1000, 201819, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(1000, null, "PublishedDate lt {0}")]
        [DataRow(1001, 201819, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(1001, null, "PublishedDate lt {0}")]
        [DataRow(9999, 201819, "PublishedDate lt {0} and year eq 201819")]
        [DataRow(9999, null, "PublishedDate lt {0}")]
        public async Task SearchProvider_CheckParameters(int? laFirstSearchSize, int? academicYear, string expectedFilter)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(new[] { new MockDocumentConfiguration(0, AzureSearchMaxResults, 1) }, null);
            var mockLaIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, 1);

            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, laFirstSearchSize);
            var beforeDateTime = new DateTime(2002, 6, 24, 3, 37, 49);
            var beforeDateTimeOffset = new DateTimeOffset(beforeDateTime).ToUniversalTime().ToString("O");

            // Act
            var result = await searchService.SearchProvider(It.IsAny<string>(), "search", beforeDateTime, academicYear);

            // Assert
            mockProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.QueryType == SearchQueryType.Full
                        && op.Facets.Count == 0
                        && op.Filter == string.Format(expectedFilter, beforeDateTimeOffset))),
                Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(null, "/.*/")]
        [DataRow("", "/.*/")]
        [DataRow(" ", "/.*/")]
        [DataRow("_", "/.*/")]
        [DataRow("-", "/.*/")]
        [DataRow("-_\u2013 ~#@'!£$%^&*()[]{}/\\`¬¦|?><,.:; ", "/.*/")]
        [DataRow("search", "/.*search.*/")]
        [DataRow("$earch", "/.*earch.*/")]
        [DataRow("test-search", "/.*test_search.*/")]
        [DataRow("test $-search_", "/.*test_search.*/")]
        [DataRow(" Test $-search _", "/.*Test_search.*/")]
        public async Task SearchProvider_CheckSearchText(string searchText, string expectedSearchText)
        {
            // Arrange
            var mockProviderIndexClient = GetMockSearchClient(new[] { new MockDocumentConfiguration(0, AzureSearchMaxResults, 1) }, null);
            var mockLaIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, 1);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchProvider(It.IsAny<string>(), searchText, It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            mockProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    expectedSearchText,
                    It.IsAny<SearchOptions>()),
                Times.AtLeastOnce);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SearchProvider_GetsAllPages()
        {
            // Arrange
            var mockLaIndexClient = GetMockSearchClient(DefaultMockDocumentConfiguration, 1);
            var mockProviderIndexClient = GetMockSearchClient(
                new[]
                {
                    new MockDocumentConfiguration(0, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults),
                    new MockDocumentConfiguration(2 * AzureSearchMaxResults, AzureSearchMaxResults, AzureSearchMaxResults - 1),
                }, null);
            var clientManager = GetMockClientManager(mockProviderIndexClient, mockLaIndexClient);
            var searchService = new AzureAllocationSearchService(clientManager.Object, ProviderIndexName, LaIndexName, LaFirstSearchSize);

            // Act
            var result = await searchService.SearchProvider(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>());

            // Assert
            mockProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && (!op.Skip.HasValue || op.Skip.Value == 0)
                        && op.Facets.Count == 0)),
                Times.Once);

            mockProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            mockProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(op =>
                        op.Size == AzureSearchMaxResults
                        && op.Skip == 2 * AzureSearchMaxResults
                        && op.Facets.Count == 0)),
                Times.Once);

            Assert.AreEqual(3 * AzureSearchMaxResults - 1, result.Documents.Count());
            Assert.IsNull(result.LaGroups);
        }

        #endregion

        #region Helper Class & Methods

        private class MockDocumentConfiguration
        {
            public MockDocumentConfiguration(int skip, int size, int documentCount)
            {
                Skip = skip;
                Size = size;
                DocumentCount = documentCount;
            }

            public int Skip { get; set; }

            public int Size { get; set; }

            public int DocumentCount { get; set; }
        }

        private Mock<IAzureSearchIndexClientManager> GetMockClientManager(
            Mock<IAzureSearchIndexClient> mockProviderIndexClient,
            Mock<IAzureSearchIndexClient> mockLaIndexClient)
        {
            var mockClientManager = new Mock<IAzureSearchIndexClientManager>();

            mockClientManager.Setup(m => m.GetSearchIndexClient(ProviderIndexName)).Returns(mockProviderIndexClient.Object);
            mockClientManager.Setup(m => m.GetSearchIndexClient(LaIndexName)).Returns(mockLaIndexClient.Object);

            return mockClientManager;
        }

        private Mock<IAzureSearchIndexClient> GetMockSearchClient(MockDocumentConfiguration[] resultsConfigs, int? laGroupFacetCount)
        {
            var mockClient = new Mock<IAzureSearchIndexClient>();
            var facets = laGroupFacetCount.HasValue ? GetMockLaGroups(laGroupFacetCount.Value) : null;

            foreach (var resultsConfig in resultsConfigs)
            {
                var documents = GetMockDocuments(resultsConfig.Skip, resultsConfig.DocumentCount);

                var includedFacets = new List<string>();

                if (facets != null && resultsConfig.Skip == 0)
                {
                    includedFacets = facets;
                }

                mockClient
                    .Setup(c => c.SearchDocumentsAndLaGroups<AzureAllocationSearchDocument>(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<SearchOptions>(o =>
                            o.Size == resultsConfig.Size &&
                            (o.Skip == resultsConfig.Skip || resultsConfig.Skip == 0 && o.Skip == null))))
                        .ReturnsAsync(new object[] { documents, includedFacets });

                mockClient
                    .Setup(c => c.SearchDocumentsAsync<AzureAllocationSearchDocument>(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(o =>
                            o.Size == resultsConfig.Size &&
                            (o.Skip == resultsConfig.Skip || resultsConfig.Skip == 0 && o.Skip == null))))
                    .ReturnsAsync(documents);
            }

            return mockClient;
        }

        private List<AzureAllocationSearchDocument> GetMockDocuments(int skip, int numberRequired)
        {
            var result = new List<AzureAllocationSearchDocument>(numberRequired);

            for (int i = 1; i <= numberRequired; i++)
            {
                result.Add(new AzureAllocationSearchDocument
                {
                    Ukprn = (skip + i).ToString()
                });
            }

            return result;
        }

        private List<string> GetMockLaGroups(int numberRequired)
        {
            var result = new List<string>(numberRequired);

            for (int i = 1; i <= numberRequired; i++)
            {
                result.Add($"{{\"LaCode\": {100 + i}, \"LaName\":\"MockLa{i}\"}}");
            }

            return result;
        }

        #endregion
    }
}