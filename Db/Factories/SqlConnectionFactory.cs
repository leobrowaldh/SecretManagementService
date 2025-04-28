using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Db.Factories;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly SmsDbContext _dbContext;

    public SqlConnectionFactory(SmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public SqlConnection CreateConnection()
    {
        var connection = (SqlConnection)_dbContext.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
        return connection;
    }
}