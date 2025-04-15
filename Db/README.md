# Secret Management Service Database

# Introduction

This database is used by the Secret Management Service (SMS) to store and retrieve client contact information, 
secret notification information, and other related data.

The resource to deploy is an azure SQL database with Transparent Data Encrypription (TDE) and Allways Encrypted with Secure Enclaves.
This means that not only is the clients contact information encrypted at rest in the server with TDE, but also in transit, so that only the
client can see it by using the master column key to decrypt the column keys for Emails and Phone numbers. Not even the database administrators
can see the contact information of the clients, only those who have access to the Column Master Key can see the data.

Besides that the database has role based access with roles for the function app, the client users and the client external administrators. 
All this is set to work with Azure Entra Id to authenticate users and applications.
The function app can access all the data in the database, in order to fetch the contact information and update the notification dates.
The dbo can also access all the data in the database, but cannot see emails or phone numbers if he dont have the master column key.
The client users and the external administrators are restricted by row level security so that they only access their own companys data.
This is filtered by SubscriberId that represent their company. The client users can only access the Secrets table, in order to update the secrets
when notified, and the clients external administrators can access all tables of their company.

# Setting up the database resource.

There is really no point in creating a local database, since everything is set to work in the cloud.

The resource to create is an Azure Sql database with Transparent Data Encryption (TDE) and Allways Encrypted with Secure Enclaves enabled.
Virtualization based security (VBS) was used for the enclaves, but you could use Intel SGX enclaves if you want to try it out.

Next you need to create the encryption keys. In MSSQL, connect to the database using Entra with an account that has the role of dbo.
Then expand the database and go to the Security section.
You should see a folder called "Allways Encrypted Keys". Right click on it and select "New Column Master Key".
Make sure you have the permissions, and create this key in your keyvault (Through MSSQL)
Next right click on the "Column Encryption Keys" folder and select "New Column Encryption Key", and link it to the allready created Master Key.
You can also create the keys with an SQL script.

Next disconnect from the database, and connect again, this time enabling Allways encrypted and secure Enclaves. If using VBS, use attestation=None.

# The Connection String:

Use the following format in your connection string:
```"Server=<your server>;Initial Catalog=<your database>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;Column Encryption Setting=Enabled;Attestation Protocol=None"```

Save your connection string in keyvault under the name "ConnectionStrings--SecretManagementServiceContext".

# Running the scripts and running the migrations:

Run the scripts in the following order:

1. [create the schemas] (./SqlScripts/initDatabase-pre-migrations.sql)
2. Run the efc migrations to the database to create the tables (update-database in the package manager)
3. [creating users, roles, rbs, ] (./SqlScripts/initDatabase-post-migrations.sql)
4. (Optional) [seed test data] (./SqlScripts/seedData.sql)]
5. [encrypt the columns] (./SqlScripts/ColumnEncryption.sql)

To person encrypting the columns of course need access to the column master key.