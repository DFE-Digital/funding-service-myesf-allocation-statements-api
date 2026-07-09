namespace AllocationStatementsApi.Services.Helpers
{
    /// <summary>
    /// String extension class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the Search Service Uri.
        /// </summary>
        /// <param name="serviceName">The AllocationsAzureSearchService Name from the appsettings.</param>
        /// <returns>The URI for the Azure Search Index Client.</returns>
        public static Uri GetSearchServiceUri(this string serviceName)
        {
            return new Uri($"https://{serviceName}.search.windows.net/");
        }
    }
}