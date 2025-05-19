using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Azure.Identity;

namespace Db.Helpers;

public static class SqlAlwaysEncryptedConfig
{
    /// <summary>
    /// Configures the keyvault access for fetching the master key
    /// </summary>
    public static void RegisterAzureKeyVaultProvider()
    {
        var akvProvider = new SqlColumnEncryptionAzureKeyVaultProvider(new DefaultAzureCredential());

        SqlConnection.RegisterColumnEncryptionKeyStoreProviders(
            customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>
            {
                { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider }
            });
    }
}
