using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace SMSFunctionApp.Helpers;

/// <summary>
/// Ideal for those pesky free db that fall asleep after a while.
/// </summary>
public static class RetryHelper
{
    public static async Task ExecuteWithSqlRetryAsync(Func<Task> operation, ILogger logger, int maxRetries = 3, int delayMilliseconds = 5000)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await operation();
                return; // Success
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Message.Contains("post-login phase"))
            {
                logger.LogWarning("Azure SQL cold start delay. Attempt {attempt}/{maxRetries}.", attempt, maxRetries);

                if (attempt == maxRetries)
                {
                    logger.LogError("Max retry attempts reached. Giving up.");
                    throw;
                }

                await Task.Delay(delayMilliseconds);
            }
        }
    }

    public static async Task<T> ExecuteWithSqlRetryAsync<T>(Func<Task<T>> operation, ILogger logger, int maxRetries = 3, int delayMilliseconds = 5000)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var result = await operation();
                return result; // Success
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Message.Contains("post-login phase"))
            {
                logger.LogWarning("Azure SQL cold start delay. Attempt {attempt}/{maxRetries}.", attempt, maxRetries);

                if (attempt == maxRetries)
                {
                    logger.LogError("Max retry attempts reached. Giving up.");
                    throw;
                }

                await Task.Delay(delayMilliseconds);
            }
        }
        throw new Exception("Operation failed after maximum retry attempts.");
    }
}

