using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Azure.Identity;

namespace Db.Helpers;
//Configs that are only needed when running locally, azure takes care of it when deployed.
public static class SqlAlwaysEncryptedLocalConfig
{
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
