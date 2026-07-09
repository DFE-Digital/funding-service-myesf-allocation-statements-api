namespace AllocationStatementsApi.Services.Configuration
{
    /// <summary>
    /// The Allocations CosmosDb repository configuration.
    /// </summary>
    public class AllocationsCosmosDbConfiguration
    {
        /// <summary>
        /// Gets or sets the service endpoint.
        /// </summary>
        public string ServiceEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the authorisation key or resource token.
        /// </summary>
        public string AuthKeyOrResourceToken { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the collection name.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the policy for the maximum number of retries
        /// to attempt when Cosmos DB throttles requests.
        /// </summary>
        public int MaxRetryAttemptsOnThrottledRequests { get; set; }

        /// <summary>
        /// Gets or sets the policy for the maximum cumulative retry wait time
        /// to use when Cosmos DB throttles requests.
        /// </summary>
        public int MaxRetryWaitTimeInSeconds { get; set; }
    }
}