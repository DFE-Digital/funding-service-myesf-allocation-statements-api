using AllocationStatementsApi.Services.Implementations.IFileMetadata.Models;
using AllocationStatementsApi.Services.Interfaces;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Pds.Core.Logging;
using System.Reflection;
using Type = AllocationStatementsApi.Services.Implementations.IFileMetadata.Models.Type;

namespace AllocationStatementsApi.Services.Implementations.IFileMetadata
{
    /// <summary>
    /// An implementation of IAllocationRepository that uses CosmosDbRepository as the 'database'.
    /// </summary>
    public class CosmosDbRepository : IAllocationRepository
    {
        private readonly ILoggerAdapter<CosmosDbRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbRepository"/> class.
        /// Creates a service to store and read metadata info from an Azure CosmosDBRepository.
        /// </summary>
        /// <param name="databaseName">The name of the DB CosmosDBRepository.</param>
        /// <param name="collectionName">The collection we wish to use.</param>
        /// <param name="endpoint">The URI of the service.</param>
        /// <param name="accountKey">The account key.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="maxRetryAttemptsOnThrottledRequests">The max number of attempts on throttled requests.</param>
        /// <param name="maxRetryWaitTimeInSeconds">The max seconds to wait before retrying.</param>
        public CosmosDbRepository(
                                  string databaseName,
                                  string collectionName,
                                  string endpoint,
                                  string accountKey,
                                  ILoggerAdapter<CosmosDbRepository> logger,
                                  int maxRetryAttemptsOnThrottledRequests = 100,
                                  int maxRetryWaitTimeInSeconds = 120)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
            Endpoint = endpoint;
            AccountKey = accountKey;
            _logger = logger;
            _maxRetryAttemptsOnThrottledRequests = maxRetryAttemptsOnThrottledRequests;
            _maxRetryWaitTimeInSeconds = maxRetryWaitTimeInSeconds;
        }

        #region Private properties

        /// <summary>
        /// Gets or sets the name of the DB CosmosDBRepository.
        /// </summary>
        private string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the collection we wish to use.
        /// </summary>
        private string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the URI of the service.
        /// </summary>
        private string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the account key.
        /// </summary>
        private string AccountKey { get; set; }

        /// <summary>
        /// The document client.
        /// </summary>
        private CosmosClient _client = null!;

        /// <summary>
        /// Gets the Container.
        /// </summary>
        private Container Container => Client.GetContainer(DatabaseName, CollectionName);

        /// <summary>
        /// The max number of retries to attempt when rate limited.
        /// </summary>
        private readonly int _maxRetryAttemptsOnThrottledRequests;

        /// <summary>
        /// The max amount of time to wait in total when rate limited.
        /// </summary>
        private readonly int _maxRetryWaitTimeInSeconds;

        private CosmosClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new CosmosClient(
                        Endpoint,
                        AccountKey,
                        new CosmosClientOptions
                        {
                            AllowBulkExecution = true,
                            ConnectionMode = ConnectionMode.Gateway,
                            MaxRetryAttemptsOnRateLimitedRequests = _maxRetryAttemptsOnThrottledRequests,
                            MaxRetryWaitTimeOnRateLimitedRequests = new TimeSpan(0, 0, _maxRetryWaitTimeInSeconds)
                        });
                    return _client;
                }

                return _client;
            }
        }

        #endregion Private properties


        #region Public Methods

        /// <summary>
        /// Gets the allocation statement by identifier.
        /// </summary>
        /// <param name="id">The identifier. </param>
        /// <param name="ukprn">The ukprn. </param>
        /// <returns>Returns back the allocation.</returns>
        public async Task<List<IType>> GetAllocationStatementById(string id, string ukprn)
        {
            try
            {
                return await GetAllocationStatementById_Execute(id, ukprn);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("GetAllocationStatementById", "getAllocationStatementById.js");
                    return await GetAllocationStatementById_Execute(id, ukprn);
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the allocation statements by ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn. </param>
        /// <returns>Returns back the allocations.</returns>
        public async Task<List<IType>> GetAllocationStatementsByUkprn(string ukprn)
        {
            try
            {
                return await GetAllocationStatementsByUkprn_Execute(ukprn);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("GetAllocationStatementsByUkprn", "getAllocationStatementsByUkprn.js");
                    return await GetAllocationStatementsByUkprn_Execute(ukprn);
                }

                throw;
            }
        }

        /// <summary>
        /// Gets back if the allocation statement exists for a given ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn. </param>
        /// <returns>Returns true if allocation exists for ukprn.</returns>
        public async Task<bool> GetAllocationStatementsExistsByUkprn(string ukprn)
        {
            try
            {
                return await GetAllocationStatementsExistsByUkprn_Execute(ukprn);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("GetAllocationStatementsExistsByUkprn", "getAllocationStatementsExistsByUkprn.js");
                    return await GetAllocationStatementsExistsByUkprn_Execute(ukprn);
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the allocation statement not read for a ukprn.
        /// </summary>
        /// <param name="ukprn">The ukprn. </param>
        /// <returns>Returns back the allocations not read by a ukprn.</returns>
        public async Task<int> GetAllocationStatementsNotReadByUkprn(string ukprn, string principal)
        {
            try
            {
                return await GetAllocationStatementsNotReadByUkprn_Execute(ukprn, principal);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("GetAllocationStatementsNotReadByUkprn", "getAllocationStatementsNotReadByUkprn.js");
                    return await GetAllocationStatementsNotReadByUkprn_Execute(ukprn, principal);
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the count of allocation statements within a date range.
        /// </summary>
        /// <param name="dateFrom">The date to begin search. </param>
        /// <param name="dateTo">The date to end search. </param>
        /// <returns>Returns back the allocations not read by a ukprn.</returns>
        public Task<int> GetAllocationStatementCountByCreatedAtDateRange(string dateFrom, string dateTo)
        {
            try
            {
                return GetAllocationStatementCountByCreatedAtDateRange_Execute(dateFrom, dateTo);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Sets the allocation statement as read by identifier.
        /// </summary>
        /// <param name="id">The identifier. </param>
        /// <param name="ukprn">The ukprn. </param>
        public async Task SetAllocationAsReadById(string id, string ukprn, string principleId)
        {
            try
            {
                await SetAllocationStatementAsReadById_Execute(id, ukprn, principleId);
                _logger.LogInformation($"Set allocation successfully to Read - id: {id}, UKPRN: {ukprn}, principleId: {principleId}");
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("SetAllocationtAsReadById", "setAllocationStatementAsReadById.js");
                    await SetAllocationStatementAsReadById_Execute(id, ukprn, principleId);
                    _logger.LogInformation($"Set allocation successfully to Read - id: {id}, UKPRN: {ukprn}, principleId: {principleId}");
                }

                _logger.LogError($"[[{nameof(SetAllocationAsReadById)}] Allocation not found - id: {id}, UKPRN: {ukprn}.");

                throw;
            }
        }

        /// <summary>
        /// Adds allocation statement.
        /// </summary>
        /// <param name="type">The allocation at type level.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AddAllocationStatement(IType type)
        {
            await Container.CreateItemAsync(type, new PartitionKey(type.UKPRN));
        }

        /// <summary>
        /// Gets current allocation statement by type and year.
        /// </summary>
        /// <param name="type">The allocation type.</param>
        /// <param name="ukprn">The ukprn of the allocation statement.</param>
        /// <param name="year">The year of the allocation statement.</param>
        /// <returns>A <see cref="Task{IType}>"/> an asynchronous operation with a result of <see cref="IType"/>.</returns>
        public async Task<IType> GetCurrentAllocationStatementByTypeAndYear(string type, string ukprn, int year)
        {
            try
            {
                return await GetCurrentAllocationStatementByTypeAndYear_Execute(type, ukprn, year);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("GetCurrentAllocationStatementByTypeAndYear", "getCurrentAllocationStatementByTypeAndYear.js");
                    return await GetCurrentAllocationStatementByTypeAndYear_Execute(type, ukprn, year);
                }

                throw;
            }
        }

        /// <summary>
        /// Clears down all allocation statements and history in cosmos db.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveAllAllocationStatements()
        {
            try
            {
                await BulkDelete_Execute();
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("BulkDelete", "bulkDelete.js");
                    await BulkDelete_Execute();
                }
            }
        }

        /// <summary>
        /// Clears down all allocation statements and history in cosmos db for a specific provider.
        /// </summary>
        /// <param name="ukprn">The UKPRN of the provider.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveAllAllocationStatements(string ukprn)
        {
            try
            {
                await BulkDeleteByUkprn_Execute(ukprn);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateStoredProcedure("BulkDelete", "bulkDelete.js");
                    await BulkDeleteByUkprn_Execute(ukprn);
                }
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Changes the Throughput of the Cosmos Repository.
        /// </summary>
        /// <param name="rus">The rus.</param>
        /// <returns>A <see cref="Task{bool}"/> representing the asynchronous operation.</returns>
        public async Task<bool> ChangeThroughput(int rus)
        {
            var offer = GetCurrentThroughput();

            if (offer.Result == rus)
            {
                return false;
            }

            await Container.ReplaceThroughputAsync(rus);
            return true;
        }

        /// <summary>
        /// Gets the current throughput of the Cosmos repository.
        /// </summary>
        /// <returns>A <see cref="Task{int}"/> representing the asynchronous operation.</returns>
        public async Task<int?> GetCurrentThroughput()
        {
            return await Container.ReadThroughputAsync();
        }

        private async Task<List<IType>> GetAllocationStatementById_Execute(string id, string ukprn)
        {
            var response = await Container.Scripts.ExecuteStoredProcedureAsync<List<Type>>("GetAllocationStatementById", new PartitionKey(ukprn.ToString()), new dynamic[] { id });

            return response.Resource.Select(type => (IType)type).ToList();
        }

        private async Task<List<IType>> GetAllocationStatementsByUkprn_Execute(string ukprn)
        {
            var response = await Container.Scripts.ExecuteStoredProcedureAsync<List<Type>>("GetAllocationStatementsByUkprn", new PartitionKey(ukprn.ToString()), null);

            return response.Resource.Select(type => (IType)type).ToList();
        }

        private async Task<bool> GetAllocationStatementsExistsByUkprn_Execute(string ukprn)
        {
            var response = await Container.Scripts.ExecuteStoredProcedureAsync<bool>("GetAllocationStatementsExistsByUkprn", new PartitionKey(ukprn.ToString()), null);

            return response.Resource;
        }

        private async Task<int> GetAllocationStatementsNotReadByUkprn_Execute(string ukprn, string principal)
        {
            var response = await Container.Scripts.ExecuteStoredProcedureAsync<int>("GetAllocationStatementsNotReadByUkprn", new PartitionKey(ukprn.ToString()), new dynamic[] { principal });

            return response.Resource;
        }

        private async Task<int> GetAllocationStatementCountByCreatedAtDateRange_Execute(string dateFrom, string dateTo)
        {
            var query = new QueryDefinition(
                       "SELECT value count(1) FROM c " +
                       "WHERE c.history[0].actionDateTimeUtc >= @DateFrom " +
                       "AND c.history[0].actionDateTimeUtc < @DateTo")
                        .WithParameter("@DateFrom", dateFrom)
                        .WithParameter("@DateTo", dateTo);

            var resultSet = Container.GetItemQueryIterator<int>(query);

            return (await resultSet.ReadNextAsync()).First();
        }

        private async Task SetAllocationStatementAsReadById_Execute(string id, string ukprn, string principleId)
        {
            await Container.Scripts.ExecuteStoredProcedureAsync<bool>("SetAllocationtAsReadById", new PartitionKey(ukprn.ToString()), new dynamic[] { id, principleId });
        }

        private async Task<IType> GetCurrentAllocationStatementByTypeAndYear_Execute(string type, string ukprn, int year)
        {
            var response = await Container.Scripts.ExecuteStoredProcedureAsync<Type>("GetCurrentAllocationStatementByTypeAndYear", new PartitionKey(ukprn.ToString()), new dynamic[] { type, year });

            return (IType)response.Resource;
        }

        private async Task BulkDelete_Execute()
        {
            var query = "select c._self from c Where IS_DEFINED(c.ukprn)";
            await Container.Scripts.ExecuteStoredProcedureAsync<BulkDeleteResult>(
                "BulkDelete", PartitionKey.None, new dynamic[] { query });
        }

        private async Task BulkDeleteByUkprn_Execute(string ukprn)
        {
            var query = $"select c._self from c Where c.ukprn={ukprn}";
            await Container.Scripts.ExecuteStoredProcedureAsync<BulkDeleteResult>(
                "BulkDelete", new PartitionKey(ukprn.ToString()), new dynamic[] { query });
        }

        /// <summary>
        /// Create a create stored procedure.
        /// </summary>
        /// <param name="spName">The name of the stored procedure.</param>
        /// <param name="spFileName">The filename of the stored procedure.</param>
        /// <returns>An awaitable task.</returns>
        private async Task CreateStoredProcedure(string spName, string spFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var spBodyPath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith(spFileName));

            using (var stream = assembly.GetManifestResourceStream(spBodyPath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var spBody = reader.ReadToEnd();

                    try
                    {
                        var spItem = new StoredProcedureProperties(spName, spBody);
                        //await Container.Scripts.CreateStoredProcedureAsync(spItem, new RequestOptions());
                        await Container.Scripts.CreateStoredProcedureAsync(spItem);
                    }
                    catch (CosmosException ex)
                    {
                        if (!ex.Message.Contains("Resource with specified id, name, or unique"))
                        {
                            throw;
                        }
                    }
                }
            }
        }

        #endregion Helpers
    }
}