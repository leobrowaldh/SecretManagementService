

-- Create users
CREATE USER [func-SecretManagementService-development-001] FROM EXTERNAL PROVIDER WITH OBJECT_ID='13e2387d-f8a2-489e-b375-50703fd42e60';            -- the App
CREATE USER [AppScopedUser_BackgroundTasks] WITHOUT LOGIN;
CREATE USER [AppScopedUser_User] WITHOUT LOGIN; -- from the app, use EXECUTE AS USER = 'AppScopedUser_User';
CREATE USER [AppScopedUser_ExternalAdmin] WITHOUT LOGIN;
CREATE USER [AppScopedUser_InternalAdmin] WITHOUT LOGIN;

--create roles
CREATE ROLE InternalAdminRole; -- inmune to rls
CREATE ROLE ExternalAdminRole;
CREATE ROLE UserRole;
CREATE ROLE NotificationFunctionAppRole; -- inmune to rls

--Assign Permissions to Roles:
ALTER ROLE InternalAdminRole ADD MEMBER [AppScopedUser_InternalAdmin];
ALTER ROLE ExternalAdminRole ADD MEMBER [AppScopedUser_ExternalAdmin];
ALTER ROLE UserRole ADD MEMBER [AppScopedUser_User];
ALTER ROLE NotificationFunctionAppRole ADD MEMBER [AppScopedUser_BackgroundTasks];

-- Internal Admins get full access to everything
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::suprusr TO InternalAdminRole;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::usr TO InternalAdminRole;

-- External Admins get full access only within RLS constraints
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::suprusr TO ExternalAdminRole;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::usr TO ExternalAdminRole;

-- Users only manage secrets within RLS constraints
GRANT SELECT, INSERT, UPDATE, DELETE ON usr.Secrets TO UserRole;

--Function app read contact info and update notification Dates
GRANT SELECT, UPDATE ON SCHEMA::usr TO NotificationFunctionAppRole;
GRANT SELECT ON SCHEMA::suprusr TO NotificationFunctionAppRole;
GO


--Create Functions for RLS:

CREATE FUNCTION rls.fn_subscriber_filter(@SubscriberId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1
    WHERE 
    IS_MEMBER('InternalAdminRole') = 1
    OR IS_MEMBER('db_owner') = 1
    OR IS_MEMBER('NotificationFunctionAppRole') = 1
    OR @SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER);
GO

CREATE FUNCTION rls.fn_api_filter(@SecretId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1
    WHERE
    IS_MEMBER('InternalAdminRole') = 1
    OR IS_MEMBER('db_owner') = 1
    OR IS_MEMBER('NotificationFunctionAppRole') = 1
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
    SELECT 1
    WHERE
    IS_MEMBER('InternalAdminRole') = 1
    OR IS_MEMBER('db_owner') = 1
    OR IS_MEMBER('NotificationFunctionAppRole') = 1
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
    SELECT 1
    WHERE
    IS_MEMBER('InternalAdminRole') = 1
    OR IS_MEMBER('db_owner') = 1
    OR IS_MEMBER('NotificationFunctionAppRole') = 1
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




