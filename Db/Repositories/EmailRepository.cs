using Microsoft.Data.SqlClient;
using Db.DbModels;
using Microsoft.Extensions.Configuration;
using Db.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System;
using Db.ResponseModels;

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

    public async Task<ResponsePage<Email>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false, bool track = false)
    {
        using (var connection = new SqlConnection(_connectionString))
        {

            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                var result = new List<Email>();
                int totalCount = 0;
                filter = filter ?? "";

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
                    SELECT EmailId, EmailAddress, SubscriberId
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
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        SubscriberId = reader.GetGuid(reader.GetOrdinal("SubscriberId"))
                    });
                }

                return new ResponsePage<Email>
                {
                    DbItemsCount = totalCount,
                    PageItems = result,
                    PageNr = pageNumber,
                    PageSize = pageSize
                };
            });
        };
    }

    public async Task<List<Email>> ReadItemsAsync(bool flat, string filter, bool seeded = false, bool track = false)
    {
        using var connection = new SqlConnection(_connectionString);

        return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
        {
            var result = new List<Email>();
            filter ??= "";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            SELECT EmailId, EmailAddress, SubscriberId
            FROM suprusr.Emails
            WHERE EmailAddress LIKE @Filter
            ORDER BY EmailId";

            cmd.Parameters.AddWithValue("@Filter", $"%{filter}%");

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new Email
                {
                    EmailId = reader.GetGuid(reader.GetOrdinal("EmailId")),
                    EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                    SubscriberId = reader.GetGuid(reader.GetOrdinal("SubscriberId"))
                });
            }

            return result;
        });
    }


    public async Task<Email?> ReadItemAsync(Guid itemId, bool flat)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            return await SqlQueryInjector.RunWithUserAsync(connection, _sessionContext, _executingUser, async () =>
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT EmailId, EmailAddress, SubscriberId FROM suprusr.Emails WHERE EmailId = @Id";
                cmd.Parameters.AddWithValue("@Id", itemId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Email
                    {
                        EmailId = reader.GetGuid(reader.GetOrdinal("EmailId")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        SubscriberId = reader.GetGuid(reader.GetOrdinal("SubscriberId"))
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
                INSERT INTO suprusr.Emails (EmailId, EmailAddress, SubscriberId)
                VALUES (@EmailId, @EmailAddress, @SubscriberId)";

                cmd.Parameters.AddWithValue("@EmailId", email.EmailId);
                cmd.Parameters.AddWithValue("@EmailAddress", email.EmailAddress);
                cmd.Parameters.AddWithValue("@SubscriberId", email.SubscriberId);

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
                SET EmailAddress = @EmailAddress, SubscriberId = @SubscriberId
                WHERE EmailId = @EmailId";

                cmd.Parameters.AddWithValue("@EmailId", email.EmailId);
                cmd.Parameters.AddWithValue("@EmailAddress", email.EmailAddress);
                cmd.Parameters.AddWithValue("@SubscriberId", email.SubscriberId);

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
                var email = await ReadItemAsync(emailId, flat: true) ?? throw new ArgumentException($"Email {emailId} not found");
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM suprusr.Emails WHERE EmailId = @EmailId";
                cmd.Parameters.AddWithValue("@EmailId", emailId);

                await cmd.ExecuteNonQueryAsync();
                return email;
            });
        }
    }

}
