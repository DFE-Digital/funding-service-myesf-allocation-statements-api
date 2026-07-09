namespace AllocationStatementsApi.Services.Configuration
{
    public class AllocationsAzureSearchServiceConfiguration
    {
        public string Name { get; set; }

        public string QueryApiKey { get; set; }

        public string LocalAuthoritySearchIndexName { get; set; }

        public string ProviderSearchIndexName { get; set; }

        public string LocalAuthoritySearchFirstPageSize { get; set; }
    }
}