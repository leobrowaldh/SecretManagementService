using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Db.Factories;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly SmsDbContext _dbContext;
    private Dictionary<string, object?>? _sessionContext;

    public SqlConnectionFactory(SmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void SetSessionContext(Dictionary<string, object?> contextVariables)
    {
        _sessionContext = contextVariables;
    }

    public SqlConnection CreateConnection()
    {
        var connection = (SqlConnection)_dbContext.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        if (_sessionContext != null)
        {
            foreach (var pair in _sessionContext)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "EXEC sp_set_session_context @key, @value";
                cmd.Parameters.AddWithValue("@key", pair.Key);
                cmd.Parameters.AddWithValue("@value", pair.Value ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        return connection;
    }
}
