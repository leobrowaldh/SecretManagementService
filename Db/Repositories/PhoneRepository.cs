using Microsoft.Data.SqlClient;
using Db.DbModels;
using Db.Dtos;
using Microsoft.Extensions.Configuration;

namespace Db.Repositories;

public class PhoneRepository : IGenericRepository<Phone>
{
    private readonly string _connectionString;

    public PhoneRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SecretManagementServiceContext") ??
            throw new InvalidOperationException("no connection string found");
    }

    public async Task<ResponsePageDto<Phone>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false)
    {
        var result = new List<Phone>();
        int totalCount = 0;
        filter = filter ?? "";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using (var countCmd = connection.CreateCommand())
        {
            countCmd.CommandText = @"
                SELECT COUNT(*) FROM suprusr.Phones
                WHERE PhoneNumber LIKE @Filter";
            countCmd.Parameters.AddWithValue("@Filter", $"%{filter}%");
            totalCount = (int)(await countCmd.ExecuteScalarAsync());
        }

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT PhoneId, PhoneNumber
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
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"))
            });
        }

        return new ResponsePageDto<Phone>
        {
            DbItemsCount = totalCount,
            PageItems = result,
            PageNr = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Phone?> ReadItemAsync(Guid itemId, bool flat)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT PhoneId, PhoneNumber FROM suprusr.Phones WHERE PhoneId = @Id";
        cmd.Parameters.AddWithValue("@Id", itemId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Phone
            {
                PhoneId = reader.GetGuid(reader.GetOrdinal("PhoneId")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"))
            };
        }

        return null;
    }

    public async Task<Phone> AddItemAsync(Phone phone)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO suprusr.Phones (PhoneId, PhoneNumber)
            VALUES (@PhoneId, @PhoneNumber)";

        cmd.Parameters.AddWithValue("@PhoneId", phone.PhoneId);
        cmd.Parameters.AddWithValue("@PhoneNumber", phone.PhoneNumber);

        await cmd.ExecuteNonQueryAsync();
        return phone;
    }

    public async Task<Phone> UpdateItemAsync(Phone phone)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            UPDATE suprusr.Phones
            SET PhoneNumber = @PhoneNumber
            WHERE PhoneId = @PhoneId";

        cmd.Parameters.AddWithValue("@PhoneId", phone.PhoneId);
        cmd.Parameters.AddWithValue("@PhoneNumber", phone.PhoneNumber);

        await cmd.ExecuteNonQueryAsync();
        return phone;
    }

    public async Task<Phone> DeleteItemAsync(Guid phoneId)
    {
        var phone = await ReadItemAsync(phoneId, flat: true);
        if (phone == null)
            throw new ArgumentException($"Phone {phoneId} not found");

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM suprusr.Phones WHERE PhoneId = @PhoneId";
        cmd.Parameters.AddWithValue("@PhoneId", phoneId);

        await cmd.ExecuteNonQueryAsync();
        return phone;
    }
}
