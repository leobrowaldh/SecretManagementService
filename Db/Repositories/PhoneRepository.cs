using Db.DbModels;
using Db.Helpers;
using Db.ResponseModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Db.Repositories;

public class PhoneRepository : GenericRepository<Phone>
{
    public PhoneRepository(SmsDbContext dbContext) : base(dbContext) { }

    //ADO.NET implementation of ReadItemsAsync is needed to filter the encrypted PhoneNumber field in the secure enclave.
    public override async Task<ResponsePage<Phone>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false, bool track = false)
    {
        var connection = (SqlConnection)_connection;

        return await SqlQueryInjector.RunWithUserAsync(_connection, _sessionContext, _executingUser, async () =>
        {
            var result = new List<Phone>();
            int totalCount = 0;
            filter = filter ?? "";

            using (var countCmd = connection.CreateCommand())
            {
                countCmd.CommandText = @"
                        SELECT COUNT(*) FROM suprusr.Phones
                        WHERE PhoneNumber LIKE @Filter";
                countCmd.Parameters.AddWithValue("@Filter", $"%{filter}%");
                totalCount = (int?)await countCmd.ExecuteScalarAsync() ?? 0;
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                    SELECT PhoneId, PhoneNumber, SubscriberId
                    FROM suprusr.Phones
                    WHERE PhoneNumber LIKE @Filter
                    ORDER BY PhoneId
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            cmd.Parameters.AddWithValue("@Filter", $"%{filter}%");
            cmd.Parameters.AddWithValue("@Offset", pageNumber * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new Phone
                {
                    PhoneId = reader.GetGuid(reader.GetOrdinal("PhoneId")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    SubscriberId = reader.GetGuid(reader.GetOrdinal("SubscriberId"))
                });
            }

            return new ResponsePage<Phone>
            {
                DbItemsCount = totalCount,
                PageItems = result,
                PageNr = pageNumber,
                PageSize = pageSize
            };
        });
    }

    public override async Task<List<Phone>> ReadItemsAsync(bool flat, string filter, bool seeded = false, bool track = false)
    {
        var connection = (SqlConnection)_connection;

        return await SqlQueryInjector.RunWithUserAsync(_connection, _sessionContext, _executingUser, async () =>
        {
            var result = new List<Phone>();
            filter ??= "";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            SELECT PhoneId, PhoneNumber, SubscriberId
            FROM suprusr.Phones
            WHERE PhoneNumber LIKE @Filter
            ORDER BY PhoneId";

            cmd.Parameters.AddWithValue("@Filter", $"%{filter}%");

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new Phone
                {
                    PhoneId = reader.GetGuid(reader.GetOrdinal("PhoneId")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    SubscriberId = reader.GetGuid(reader.GetOrdinal("SubscriberId"))
                });
            }

            return result;
        });
    }

}
