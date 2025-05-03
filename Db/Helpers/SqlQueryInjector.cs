using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Db.Helpers;

//Helper methods to modify the SQL query that works with both ADO and EFC 
public class SqlQueryInjector
{

    public static async Task<TResult> RunWithUserAsync<TResult>(
    DbConnection connection,
    Dictionary<string, object?> sessionContext,
    string executingUser,
    Func<Task<TResult>> action)
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            await ApplySessionContextAsync(connection, sessionContext);
            await ExecuteAsUserAsync(connection, executingUser);

            return await action();
        }
        finally
        {
            await RevertAsync(connection);
        }
    }


    /// <summary>
    /// Applies the session context to the connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="sessionContext"></param>
    /// <returns></returns>
    public static async Task ApplySessionContextAsync(DbConnection connection, Dictionary<string, object?> sessionContext)
    {
        if (sessionContext == null || sessionContext.Count == 0)
            return;
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        foreach (var kvp in sessionContext)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "EXEC sp_set_session_context @key = @Key, @value = @Value;";

            var keyParam = command.CreateParameter();
            keyParam.ParameterName = "@Key";
            keyParam.Value = kvp.Key;
            command.Parameters.Add(keyParam);

            var valueParam = command.CreateParameter();
            valueParam.ParameterName = "@Value";
            valueParam.Value = kvp.Value ?? DBNull.Value;
            command.Parameters.Add(valueParam);

            await command.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// Add the Execute As User command to the connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    public static async Task ExecuteAsUserAsync(DbConnection connection, string userName)
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "EXECUTE AS USER = @user;";
        var param = command.CreateParameter();
        param.ParameterName = "@user";
        param.Value = userName;
        command.Parameters.Add(param);
        await command.ExecuteNonQueryAsync();
    }
    

    public static async Task RevertAsync(DbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "REVERT;";
        await command.ExecuteNonQueryAsync();
    }
}
