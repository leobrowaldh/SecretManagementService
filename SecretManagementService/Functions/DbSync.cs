using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SecretManagementService.Services;
using SMSFunctionApp.Helpers;
using System;
using System.Threading.Tasks;

namespace SMSFunctionApp.Functions;

public class DbSync
{
    private readonly ILogger _logger;
    private readonly ISecretsService _secretsService;

    public DbSync(ILoggerFactory loggerFactory, ISecretsService secretsService)
    {
        //the loggerFactory allows you to create different kinds of loggers in the same class
        _logger = loggerFactory.CreateLogger<DbSync>();
        _secretsService = secretsService;
    }

    [Function(nameof(DbSync))]
    [FixedDelayRetry(2, "00:00:10")] // Retry 2 times with a 10-second delay between attempts
    public async Task Run([TimerTrigger("0 0 6 * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function DbSync executed at: {executionTime}", DateTime.Now);
        
        _logger.LogInformation("Syncronizing database secrets with source...");

        await RetryHelper.ExecuteWithSqlRetryAsync(() => 
            _secretsService.SyncDatabaseSecretsWithSource(),_logger);
    }
}