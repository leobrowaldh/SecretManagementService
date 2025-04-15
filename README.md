# Secret Management Service SMS

# Introduction

This application automates the renewal of secrets for your clients.
The function app takes care of spotting secrets that are about to expire, fetches the notification method from the database, 
and notifies the respective client to renew them.
The client then access our UI and renews the secret through our SMSapi.
The client can also utilize our SecretHandlingApi to automate the client side of the secret renewal process.

# The Function App

The SMSFunctionApp is an isolated Azure Functions App. 
It is responsible for the following tasks:
- Making time triggered checks against the cloud provider that manages the secrets (e.g. Microsoft Graph api) 
  and fetching the id of any secret that is about to expire, saving it in a notification queue.
- Listen to this notification queue and trigger the notification service for each expiring secret.
- The notification service fetches the notification method from the database and sends the notification to the client.

For more information on the database, please refer to the [Function App](./SMSFunctionApp/README.md) documentation.

# The Database

The database is an Azure SQL database with Transparent data encryption at rest, and Allways encrypted with secure enclaves for in transit encryption,
wich means that the contact information of the clients is unaccessible even for database administrators.
It has role based access with roles for the function app, the client users and the client external administrators.
Role based security is in place to ensure that each user or admin only accesses its own companys data.
Comunication with the database ocurrs mostly through entity framework core repositories. 
ADO.net repositories are used for querying the encrypted columns.
sql scripts are used for setting up the schemas, roles, row security filtering functions, seeding test data, 
encrypting the columns, and clearing the database to start over if needed.

For more information on the database, please refer to the [Database](./Db/README.md) documentation.