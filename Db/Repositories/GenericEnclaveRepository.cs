using System.Data;
using Microsoft.Data.SqlClient;
using Db.Dtos;
using Db.Repositories;

namespace DbRepos;

public class GenericEnclaveRepository<T> : IGenericRepository<T> where T : class, new()
{
    private readonly string _connectionString;
    private readonly string _tableName;

    public GenericEnclaveRepository(string connectionString)
    {
        _connectionString = connectionString;
        _tableName = typeof(T).Name; // Table assumed to match class name
    }

    public async Task<ResponsePageDto<T>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false)
    {
        var result = new List<T>();
        int totalCount = 0;

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var countCmd = connection.CreateCommand();
        countCmd.CommandText = $"SELECT COUNT(*) FROM [{_tableName}] WHERE 1=1"; // extend with filter logic
        totalCount = (int)(await countCmd.ExecuteScalarAsync());

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $@"
            SELECT * FROM [{_tableName}]
            ORDER BY [Id]
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        cmd.Parameters.AddWithValue("@Offset", pageNumber * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var item = MapFromReader(reader);
            result.Add(item);
        }

        return new ResponsePageDto<T>
        {
            DbItemsCount = totalCount,
            PageItems = result,
            PageNr = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<T?> ReadItemAsync(Guid itemId, bool flat)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT * FROM [{_tableName}] WHERE [Id] = @Id";
        cmd.Parameters.AddWithValue("@Id", itemId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapFromReader(reader);

        return null;
    }

    public async Task<T> AddItemAsync(T item)
    {
        throw new NotImplementedException("Insert logic must be implemented per entity.");
    }

    public async Task<T> UpdateItemAsync(T item)
    {
        throw new NotImplementedException("Update logic must be implemented per entity.");
    }

    public async Task<T> DeleteItemAsync(Guid itemId)
    {
        var item = await ReadItemAsync(itemId, true);
        if (item == null)
            throw new ArgumentException($"Item {itemId} not found");

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"DELETE FROM [{_tableName}] WHERE [Id] = @Id";
        cmd.Parameters.AddWithValue("@Id", itemId);

        await cmd.ExecuteNonQueryAsync();
        return item;
    }

    // Map SqlDataReader to T manually or with reflection
    private T MapFromReader(SqlDataReader reader)
    {
        var obj = new T();
        foreach (var prop in typeof(T).GetProperties())
        {
            if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull)
                continue;

            prop.SetValue(obj, reader[prop.Name]);
        }
        return obj;
    }

    public Task SetContextAsync(Dictionary<string, object> contextVariables)
    {
        throw new NotImplementedException();
    }
}

// Extension method to safely check columns
public static class SqlReaderExtensions
{
    public static bool HasColumn(this IDataRecord reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
