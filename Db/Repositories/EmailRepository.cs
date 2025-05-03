using Microsoft.Data.SqlClient;
using Db.DbModels;
using Db.Dtos;
using Microsoft.Extensions.Configuration;
using Db.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Db.Repositories;

public class EmailRepository : IEmailRepository
{
    private readonly string _connectionString;
    private Dictionary<string, object?> _sessionContext = new();
    private string _executingUser = string.Empty;

    public EmailRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SecretManagementServiceContext")
            ?? throw new InvalidOperationException("Missing Connection string.");
    }

    public void SetContext(Dictionary<string, object?> contextVariables)
    {
        _sessionContext = contextVariables ?? new();
    }

    public void SetExecutingUser(string executingUser)
    {
        _executingUser = executingUser;
    }

    public async Task<ResponsePageDto<Email>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false)
    {
        using (var connection = new SqlConnection(_connectionString))
        {

            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                var result = new List<Email>();
                int totalCount = 0;
                filter = filter ?? "";

                await connection.OpenAsync();
                await SqlQueryInjector.ApplySessionContextAsync(connection, _sessionContext);
                await SqlQueryInjector.ExecuteAsUserAsync(connection, _executingUser);

                using (var countCmd = connection.CreateCommand())
                {
                    countCmd.CommandText = @"
                        SELECT COUNT(*) FROM suprusr.Emails
                        WHERE EmailAddress LIKE @Filter";
                    countCmd.Parameters.AddWithValue("@Filter", $"%{filter}%");
                    totalCount = (int)(await countCmd.ExecuteScalarAsync());
                }

                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT EmailId, EmailAddress
                    FROM suprusr.Emails
                    WHERE EmailAddress LIKE @Filter
                    ORDER BY EmailId
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                cmd.Parameters.AddWithValue("@Filter", $"%{filter}%");
                cmd.Parameters.AddWithValue("@Offset", pageNumber * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new Email
                    {
                        EmailId = reader.GetGuid(reader.GetOrdinal("EmailId")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress"))
                    });
                }

                return new ResponsePageDto<Email>
                {
                    DbItemsCount = totalCount,
                    PageItems = result,
                    PageNr = pageNumber,
                    PageSize = pageSize
                };
            });
        };
    }

    public async Task<Email?> ReadItemAsync(Guid itemId, bool flat)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT EmailId, EmailAddress FROM suprusr.Emails WHERE EmailId = @Id";
                cmd.Parameters.AddWithValue("@Id", itemId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Email
                    {
                        EmailId = reader.GetGuid(reader.GetOrdinal("EmailId")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress"))
                    };
                }

                return null;
            });
        }
    }

    public async Task<Email> AddItemAsync(Email email)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO suprusr.Emails (EmailId, EmailAddress)
                VALUES (@EmailId, @EmailAddress)";

                cmd.Parameters.AddWithValue("@EmailId", email.EmailId);
                cmd.Parameters.AddWithValue("@EmailAddress", email.EmailAddress);

                await cmd.ExecuteNonQueryAsync();
                return email;
            });
        }
    }

    public async Task<Email> UpdateItemAsync(Email email)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                UPDATE suprusr.Emails
                SET EmailAddress = @EmailAddress
                WHERE EmailId = @EmailId";

                cmd.Parameters.AddWithValue("@EmailId", email.EmailId);
                cmd.Parameters.AddWithValue("@EmailAddress", email.EmailAddress);

                await cmd.ExecuteNonQueryAsync();
                return email;
            });
        }
    }

    public async Task<Email> DeleteItemAsync(Guid emailId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                var email = await ReadItemAsync(emailId, flat: true);
                if (email == null)
                    throw new ArgumentException($"Email {emailId} not found");

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM suprusr.Emails WHERE EmailId = @EmailId";
                cmd.Parameters.AddWithValue("@EmailId", emailId);

                await cmd.ExecuteNonQueryAsync();
                return email;
            });
        }
    }

    public async Task<List<Email>> GetEmailsBySecretIdAsync(Guid secretId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                var emails = new List<Email>();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT e.EmailId, e.EmailAddress
                FROM suprusr.Emails e
                INNER JOIN suprusr.EmailSecret es ON e.EmailId = es.EmailsEmailId
                WHERE es.SecretsSecretId = @SecretId";

                cmd.Parameters.AddWithValue("@SecretId", secretId);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    emails.Add(new Email
                    {
                        EmailId = reader.GetGuid(reader.GetOrdinal("EmailId")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress"))
                    });
                }

                return emails;
            });
        }
    }
}
