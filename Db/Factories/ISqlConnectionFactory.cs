using Microsoft.Data.SqlClient;

namespace Db.Factories;
public interface ISqlConnectionFactory
{
    void SetSessionContext(Dictionary<string, object?> contextVariables);
    SqlConnection CreateConnection();
}

