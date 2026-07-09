using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AllocationStatementsApi.Services.Implementations.IAllocationSearchService
{
    /// <summary>
    /// An implementation of IAllocationSearchService that uses Azure Search.
    /// </summary>
    public class AzureAllocationSearchService : Interfaces.IAllocationSearchService.IAllocationSearchService
    {
        #region Fields & Constants

        /// <summary>
        /// This is a hard limit on the number of results that Azure Search will return per request.
        /// </summary>
        private const int MaximumResultsPerAzureSearch = 1000;
        private const string DefaultSearchPattern = "/.*/";

        // Property names:
        private static readonly string LocalAuthorityGroupPropertyName = GetJsonSerializedPropertyName(
            typeof(AzureAllocationSearchDocument), nameof(AzureAllocationSearchDocument.LaGroup));

        private static readonly string PublishedDatePropertyName = GetJsonSerializedPropertyName(typeof(AzureAllocationSearchDocument), nameof(AzureAllocationSearchDocument.PublishedDateTime));
        private static readonly string YearPropertyName = GetJsonSerializedPropertyName(typeof(AzureAllocationSearchDocument), nameof(AzureAllocationSearchDocument.Year));

        // Regular Expressions for processing the search text:
        private static readonly Regex _spacingCharacters = new Regex(@"[\s\u2212\u2013\u2014\u2010-]+", RegexOptions.Compiled);
        private static readonly Regex _disallowedCharacters = new Regex(@"[^\w]+", RegexOptions.Compiled);
        private static readonly Regex _multipleDashes = new Regex(@"_{2,}", RegexOptions.Compiled);

        private readonly IAzureSearchIndexClientManager _searchIndexClientManager;
        private readonly string _localAuthoritySearchIndexName, _providerSearchIndexName;
        private readonly int? _localAuthoritySearchFirstPageSize;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureAllocationSearchService"/> class.
        /// Create a new instance of an AzureAllocationSearchService.
        /// </summary>
        /// <param name="searchIndexClientManager">The Search Index Client manager to use.</param>
        /// <param name="providerSearchIndexName">The name of the provider search index.</param>
        /// <param name="localAuthoritySearchIndexName">The name of the LA search index.</param>
        /// <param name="localAuthoritySearchFirstPageSize">If not null and within range, the number of LA search results to get on the first request.</param>
        public AzureAllocationSearchService(
            IAzureSearchIndexClientManager searchIndexClientManager,
            string providerSearchIndexName,
            string localAuthoritySearchIndexName,
            int? localAuthoritySearchFirstPageSize)
        {
            _searchIndexClientManager = searchIndexClientManager;
            _providerSearchIndexName = providerSearchIndexName;
            _localAuthoritySearchIndexName = localAuthoritySearchIndexName;
            _localAuthoritySearchFirstPageSize = localAuthoritySearchFirstPageSize;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Search for a Local Authority.
        /// </summary>
        /// <param name="allocationType">(Not used) The allocation type to find.</param>
        /// <param name="searchTerm">The search term string to lookup.</param>
        /// <param name="beforeDateTime">Only find azureSearchResult published before this date and time.</param>
        /// <param name="year">The academic year to lookup.</param>
        /// <param name="customFilter">(Optional) A function that will be applied to filter the results that are returned.</param>
        /// <returns>A search result object containing the matching Local Authorities and some or all of their corresponding providers' azureSearchResult.</returns>
        public async Task<IAllocationSearchResult> SearchLocalAuthority(string allocationType, string searchTerm, DateTime beforeDateTime, int? year, Func<IAllocationSearchDocument, bool>? customFilter = null)
        {
            IAzureSearchIndexClient searchQueryClient = _searchIndexClientManager.GetSearchIndexClient(_localAuthoritySearchIndexName);
            SearchOptions parameters = GetSearchParameters(year, beforeDateTime, true);

            // Optimise the first request if we might not want all of the allocations back:
            if (customFilter == null
                && _localAuthoritySearchFirstPageSize.HasValue
                && _localAuthoritySearchFirstPageSize.Value >= 0
                && _localAuthoritySearchFirstPageSize.Value < MaximumResultsPerAzureSearch)
            {
                parameters.Size = _localAuthoritySearchFirstPageSize.Value;
            }

            string cleanSearchTerm = GetCleanSearchTerm(searchTerm);

            var documentsAndLaGroups = await searchQueryClient.SearchDocumentsAndLaGroups
                <AzureAllocationSearchDocument>(cleanSearchTerm, LocalAuthorityGroupPropertyName, parameters);

            var azureSearchResult = (List<AzureAllocationSearchDocument>)documentsAndLaGroups[0];
            var documents = (List<AzureAllocationSearchDocument>)documentsAndLaGroups[0];
            var laGroups = (List<string>)documentsAndLaGroups[1];

            // Get the remaining pages of allocations if we need them:
            var currentSkip = 0;
            while ((customFilter != null || laGroups.Count() == 1) && azureSearchResult.Count >= parameters.Size)
            {
                parameters = GetSearchParameters(year, beforeDateTime, false);

                currentSkip += azureSearchResult.Count;
                parameters.Skip = currentSkip;

                azureSearchResult =
                    await searchQueryClient.SearchDocumentsAsync<AzureAllocationSearchDocument>(cleanSearchTerm, parameters);

                documents.AddRange(azureSearchResult);
            }

            return new AzureAllocationSearchResult
            {
                Documents = customFilter == null
                    ? documents
                    : documents.Where(customFilter),
                LaGroups = laGroups
            };
        }

        /// <summary>
        /// Search for a provider.
        /// </summary>
        /// <param name="allocationType">(Not used) The allocation type to find.</param>
        /// <param name="searchTerm">The search term string to lookup.</param>
        /// <param name="beforeDateTime">Only find azureSearchResult published before this date and time.</param>
        /// <param name="year">The academic year to lookup.</param>
        /// <returns>A search result object containing all of the matching providers' azureSearchResult.</returns>
        public async Task<IAllocationSearchResult> SearchProvider(string allocationType, string searchTerm, DateTime beforeDateTime, int? year)
        {
            IAzureSearchIndexClient searchQueryClient = _searchIndexClientManager.GetSearchIndexClient(_providerSearchIndexName);

            SearchOptions parameters = GetSearchParameters(year, beforeDateTime, false);
            var cleanSearchTerm = GetCleanSearchTerm(searchTerm);

            var azureSearchResult = await searchQueryClient.SearchDocumentsAsync<AzureAllocationSearchDocument>(cleanSearchTerm, parameters);

            var allocations = azureSearchResult;

            // Get the remaining pages of allocations if any more exist:
            var currentSkip = 0;
            while (azureSearchResult.Count >= MaximumResultsPerAzureSearch)
            {
                parameters = GetSearchParameters(year, beforeDateTime, false);

                currentSkip += azureSearchResult.Count;
                parameters.Skip = currentSkip;

                azureSearchResult = await searchQueryClient.SearchDocumentsAsync<AzureAllocationSearchDocument>(cleanSearchTerm, parameters);

                allocations.AddRange(azureSearchResult);
            }

            return new AzureAllocationSearchResult
            {
                Documents = allocations
            };
        }

        #endregion


        #region Private Methods

        private static string GetJsonSerializedPropertyName(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);

            if (property == null)
            {
                throw new Exception("Property does not exist");
            }

            return property.GetCustomAttribute<JsonPropertyAttribute>()?
                .PropertyName
                ?? propertyName;
        }

        private string GetCleanSearchTerm(string originalSearchTerm)
        {
            if (string.IsNullOrEmpty(originalSearchTerm))
            {
                return DefaultSearchPattern;
            }

            var cleanSearchTerm = _spacingCharacters.Replace(originalSearchTerm, "_");
            cleanSearchTerm = _disallowedCharacters.Replace(cleanSearchTerm, string.Empty);
            cleanSearchTerm = _multipleDashes.Replace(cleanSearchTerm, "_");
            cleanSearchTerm = cleanSearchTerm.Trim(new[] { '_' });

            if (cleanSearchTerm == string.Empty)
            {
                return DefaultSearchPattern;
            }

            // Lucene regex pattern with wildcards at start and end of the search string:
            return $"/.*{cleanSearchTerm}.*/";
        }

        private SearchOptions GetSearchParameters(int? year, DateTime beforeDateTime, bool includeFacets)
        {
            var result = new SearchOptions()
            {
                Filter = BuildFilter(year, beforeDateTime),
                QueryType = SearchQueryType.Full,
                Size = MaximumResultsPerAzureSearch,
                IncludeTotalCount = true
            };

            var laGroupPropertyName = $"{LocalAuthorityGroupPropertyName},count:1000000";

            if (includeFacets && !result.Facets.Contains(laGroupPropertyName))
            {
                result.Facets.Add(laGroupPropertyName);
            }

            return result;
        }

        private string BuildFilter(int? year, DateTime beforeDateTime)
        {
            var filterBuilder = new StringBuilder();
            var beforeDateTimeOffset = new DateTimeOffset(beforeDateTime).ToUniversalTime().ToString("O");

            filterBuilder.Append($"{PublishedDatePropertyName} lt {beforeDateTimeOffset}");

            if (year.HasValue && year.Value > 0)
            {
                filterBuilder.Append($" and {YearPropertyName} eq {year.Value}");
            }

            return filterBuilder.ToString();
        }

        #endregion
    }
}