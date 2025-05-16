using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SecretManagementService.Services;

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
    public async Task Run([TimerTrigger("0 0 6 * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function DbSync executed at: {executionTime}", DateTime.Now);
        
        _logger.LogInformation("Syncronizing database secrets with source...");
        await _secretsService.SyncDatabaseSecretsWithSource();

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}