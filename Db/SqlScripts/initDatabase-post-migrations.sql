

-- Create users mapped to Entra ID groups (replace with actual Entra group names)
CREATE USER [SecretManagement_ExternalAdministrators] FROM EXTERNAL PROVIDER;
CREATE USER [SecretManagement_Users] FROM EXTERNAL PROVIDER;
CREATE USER [SecretManagementService-FunctionApp] FROM EXTERNAL PROVIDER;
CREATE USER [leo.browaldh@innofactor.com] FROM EXTERNAL PROVIDER;


--create roles
CREATE ROLE ExternalAdminRole;
CREATE ROLE UserRole;
CREATE ROLE AppRole;

--Assign Entra-based users to roles
ALTER ROLE ExternalAdminRole ADD MEMBER [SecretManagement_ExternalAdministrators];
ALTER ROLE UserRole ADD MEMBER [SecretManagement_Users];
ALTER ROLE AppRole ADD MEMBER [SecretManagementService-FunctionApp];
ALTER ROLE db_securityadmin ADD MEMBER [leo.browaldh@innofactor.com];
GO

--Assign Permissions to Roles:

-- External Admins get full access only within RLS constraints
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::suprusr TO ExternalAdminRole;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::usr TO ExternalAdminRole;

--Function app read contact info and update notification Dates
GRANT SELECT, UPDATE ON SCHEMA::usr TO AppRole;
GRANT SELECT ON SCHEMA::suprusr TO AppRole;

-- Users only manage secrets within RLS constraints
GRANT SELECT, INSERT, UPDATE, DELETE ON usr.Secrets TO UserRole;
GO

--Create Functions for RLS:

CREATE FUNCTION rls.fn_subscriber_filter(@SubscriberId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS fn_subscriber_filter_result
    WHERE 
    USER_NAME() = 'dbo' 
    OR @SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER);
GO

CREATE FUNCTION rls.fn_api_filter(@SecretId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS fn_api_filter_result
    WHERE
    USER_NAME() = 'dbo' 
    OR EXISTS (
        SELECT 1
        FROM usr.Secrets s
        WHERE s.SecretId = @SecretId
          AND s.SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER)
    );
GO

CREATE FUNCTION rls.fn_email_filter(@EmailId  UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS fn_email_filter_result
    WHERE
    USER_NAME() = 'dbo' 
    OR EXISTS (
        SELECT 1
        FROM usr.Secrets s
        INNER JOIN suprusr.EmailSecret es ON s.SecretId = es.SecretsSecretId
        WHERE es.EmailsEmailId = @EmailId
          AND s.SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER)
    );
GO

CREATE FUNCTION rls.fn_phone_filter(@PhoneId  UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS fn_phone_filter_result
    WHERE
    USER_NAME() = 'dbo' 
    OR EXISTS (
        SELECT 1
        FROM usr.Secrets s
        INNER JOIN suprusr.PhoneSecret ps ON s.SecretId = ps.SecretsSecretId
        WHERE ps.PhonesPhoneId = @PhoneId
          AND s.SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER)
    );
GO





--Create and apply Security Policies for RLS:
CREATE SECURITY POLICY SubscriberSecurityPolicy
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON usr.Secrets,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON suprusr.Applications,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON suprusr.Subscribers,
ADD FILTER PREDICATE rls.fn_phone_filter(PhoneId)
    ON suprusr.Phones,
ADD FILTER PREDICATE rls.fn_email_filter(EmailId)
    ON suprusr.Emails,
ADD FILTER PREDICATE rls.fn_api_filter(SecretId)
    ON suprusr.ApiEndpoints
WITH (STATE = ON);
GO




