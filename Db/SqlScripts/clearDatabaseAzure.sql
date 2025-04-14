

-- remove roles

ALTER ROLE ExternalAdminRole DROP MEMBER [SecretManagement_ExternalAdministrators];
ALTER ROLE UserRole DROP MEMBER [SecretManagement_Users];
ALTER ROLE AppRole DROP MEMBER [SecretManagementService-FunctionApp];
ALTER ROLE db_securityadmin DROP MEMBER [leo.browaldh@innofactor.com];


DROP ROLE IF EXISTS ExternalAdminRole;
DROP ROLE IF EXISTS UserRole;
DROP ROLE IF EXISTS AppRole;
GO

--drop users
DROP USER IF EXISTS  [SecretManagement_ExternalAdministrators];
DROP USER IF EXISTS [SecretManagement_Users];
DROP USER IF EXISTS [SecretManagementService-FunctionApp];
DROP USER IF EXISTS [leo.browaldh@innofactor.com];
GO

-- Drop RLS policies
DROP SECURITY POLICY IF EXISTS SubscriberSecurityPolicy;
GO

--drop functions
DROP FUNCTION IF EXISTS rls.fn_subscriber_filter;
DROP FUNCTION IF EXISTS rls.fn_api_filter;
DROP FUNCTION IF EXISTS rls.fn_phone_filter;
DROP FUNCTION IF EXISTS rls.fn_email_filter;
GO

--drop tables in the right order not to get fk conflicts
DROP TABLE IF EXISTS suprusr.ApiEndpoints;
DROP TABLE IF EXISTS suprusr.PhoneSecret;
DROP TABLE IF EXISTS suprusr.EmailSecret;
DROP TABLE IF EXISTS suprusr.Phones;
DROP TABLE IF EXISTS suprusr.Emails;
DROP TABLE IF EXISTS usr.Secrets;
DROP TABLE IF EXISTS suprusr.Applications;
DROP TABLE IF EXISTS suprusr.Subscribers;
DROP TABLE IF EXISTS dbo.__EFMigrationsHistory;
GO


-- drop schemas
DROP SCHEMA IF EXISTS rls;
DROP SCHEMA IF EXISTS usr;
DROP SCHEMA IF EXISTS suprusr;
GO