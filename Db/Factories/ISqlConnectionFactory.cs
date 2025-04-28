using Microsoft.Data.SqlClient;

namespace Db.Factories;
public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}

