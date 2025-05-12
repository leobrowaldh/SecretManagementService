

-- remove roles

ALTER ROLE NotificationFunctionAppRole DROP MEMBER [func-SecretManagementService-development-001];

DROP ROLE IF EXISTS InternalAdminRole;
DROP ROLE IF EXISTS ExternalAdminRole;
DROP ROLE IF EXISTS UserRole;
DROP ROLE IF EXISTS NotificationFunctionAppRole;
GO

--drop users
DROP USER IF EXISTS  [func-SecretManagementService-development-001];
GO

-- Drop RLS policies
DROP SECURITY POLICY IF EXISTS SubscriberSecurityPolicy;
GO

--drop functions
DROP FUNCTION IF EXISTS rls.fn_subscriber_filter;
DROP FUNCTION IF EXISTS rls.fn_application_filter;
GO

--drop tables in the right order not to get fk conflicts
DROP TABLE IF EXISTS suprusr.ApiEndpoints;
DROP TABLE IF EXISTS suprusr.PhoneApplication;
DROP TABLE IF EXISTS suprusr.EmailApplication;
DROP TABLE IF EXISTS suprusr.Phones;
DROP TABLE IF EXISTS suprusr.Emails;
DROP TABLE IF EXISTS usr.Secrets;
DROP TABLE IF EXISTS adm.Applications;
DROP TABLE IF EXISTS adm.Subscribers;
DROP TABLE IF EXISTS dbo.__EFMigrationsHistory;
GO


-- drop schemas
DROP SCHEMA IF EXISTS rls;
DROP SCHEMA IF EXISTS usr;
DROP SCHEMA IF EXISTS suprusr;
DROP SCHEMA IF EXISTS adm;
GO