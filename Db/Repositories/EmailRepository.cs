using Db.DbModels;
using Db.Helpers;
using Db.ResponseModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Db.Repositories;

public class EmailRepository : GenericRepository<Email>
{
    public EmailRepository(SmsDbContext dbContext) : base(dbContext) { }

    //ADO.NET implementation of ReadItemsAsync is needed to filter the encrypted EmailAddress field in the secure enclave.
    public override async Task<ResponsePage<Email>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false, bool track = false)
    {
        var connection = (SqlConnection)_connection;

        return await SqlQueryInjector.RunWithUserAsync(_connection, _sessionContext, _executingUser, async () =>
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
                totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync() ?? 0);

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
    }

    public override async Task<List<Email>> ReadItemsAsync(bool flat, string filter, bool seeded = false, bool track = false)
    {
        var connection = (SqlConnection)_connection;

        return await SqlQueryInjector.RunWithUserAsync(_connection, _sessionContext, _executingUser, async () =>
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

}
