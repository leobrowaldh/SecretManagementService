

-- Create users
CREATE USER [func-SecretManagementService-development-001] FROM EXTERNAL PROVIDER WITH OBJECT_ID='13e2387d-f8a2-489e-b375-50703fd42e60';            -- the App
CREATE USER [AppScopedUser_BackgroundTasks] WITHOUT LOGIN;
CREATE USER [AppScopedUser_User] WITHOUT LOGIN; -- from the app, use EXECUTE AS USER = 'AppScopedUser_User';
CREATE USER [AppScopedUser_ExternalAdmin] WITHOUT LOGIN;
CREATE USER [AppScopedUser_InternalAdmin] WITHOUT LOGIN;

CREATE USER [leo.browaldh@innofactor.com] FROM EXTERNAL PROVIDER;

--grant impersonating permissions to the app
GRANT IMPERSONATE ON USER::[AppScopedUser_BackgroundTasks] TO [func-SecretManagementService-development-001];
GRANT IMPERSONATE ON USER::[AppScopedUser_User] TO [func-SecretManagementService-development-001];
GRANT IMPERSONATE ON USER::[AppScopedUser_ExternalAdmin] TO [func-SecretManagementService-development-001];
GRANT IMPERSONATE ON USER::[AppScopedUser_InternalAdmin] TO [func-SecretManagementService-development-001];

GRANT IMPERSONATE ON USER::[AppScopedUser_BackgroundTasks] TO [leo.browaldh@innofactor.com];

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
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::adm TO InternalAdminRole;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::suprusr TO InternalAdminRole;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::usr TO InternalAdminRole;

-- External Admins get full access to contact info and users only within RLS constraints
GRANT SELECT ON adm.Applications TO ExternalAdminRole;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::suprusr TO ExternalAdminRole;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::usr TO ExternalAdminRole;

-- Users only manage secrets within RLS constraints
GRANT SELECT, INSERT, DELETE ON SCHEMA::usr TO UserRole;
GRANT SELECT ON adm.Applications TO UserRole;

--Function app read contact info and update notification Dates, as well as fetching subscriberid
GRANT SELECT, UPDATE ON SCHEMA::usr TO NotificationFunctionAppRole;
GRANT SELECT ON SCHEMA::suprusr TO NotificationFunctionAppRole;
GRANT SELECT ON SCHEMA::adm TO NotificationFunctionAppRole;

--Granting decrypting capabilities to the managed identity
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [func-SecretManagementService-development-001];
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [func-SecretManagementService-development-001];
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [AppScopedUser_BackgroundTasks];
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [AppScopedUser_BackgroundTasks];
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [AppScopedUser_User];
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [AppScopedUser_User];
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [AppScopedUser_ExternalAdmin];
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [AppScopedUser_ExternalAdmin];
GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [AppScopedUser_InternalAdmin];
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [AppScopedUser_InternalAdmin];

GRANT VIEW ANY COLUMN MASTER KEY DEFINITION TO [leo.browaldh@innofactor.com];
GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [leo.browaldh@innofactor.com];
GO


--Create Functions for RLS:

CREATE FUNCTION rls.fn_subscriber_filter(@SubscriberId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS Result
    WHERE 
    IS_MEMBER('InternalAdminRole') = 1
    OR IS_MEMBER('db_owner') = 1
    OR IS_MEMBER('NotificationFunctionAppRole') = 1
    OR @SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER);
GO

CREATE FUNCTION rls.fn_application_filter(@ApplicationId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS Result
    WHERE
    IS_MEMBER('InternalAdminRole') = 1
    OR IS_MEMBER('db_owner') = 1
    OR IS_MEMBER('NotificationFunctionAppRole') = 1
    OR EXISTS (
        SELECT 1
        FROM adm.Applications a
        WHERE a.ApplicationId = @ApplicationId
          AND a.SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER)
    );
GO

CREATE FUNCTION rls.fn_user_subscriber_filter(@UserId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS Result
    WHERE
    IS_MEMBER('InternalAdminRole') = 1
    OR IS_MEMBER('db_owner') = 1
    OR IS_MEMBER('NotificationFunctionAppRole') = 1
    OR EXISTS (
        SELECT 1
        FROM suprusr.SubscriberUsers su
        WHERE su.UserId = @UserId
          AND su.SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER)
    );
GO






--Create and apply Security Policies for RLS:
CREATE SECURITY POLICY SubscriberSecurityPolicy
ADD FILTER PREDICATE rls.fn_application_filter(ApplicationId)
    ON usr.Secrets,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON adm.Applications,
ADD FILTER PREDICATE rls.fn_user_subscriber_filter(UserId)
    ON suprusr.Users,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON suprusr.Phones,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON suprusr.Emails,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON suprusr.ApiEndpoints,
ADD FILTER PREDICATE rls.fn_application_filter(ApplicationId)
    ON suprusr.EmailApplications,
ADD FILTER PREDICATE rls.fn_application_filter(ApplicationId)
    ON suprusr.PhoneApplications,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON suprusr.SubscriberUsers,
ADD FILTER PREDICATE rls.fn_subscriber_filter(SubscriberId)
    ON adm.Subscribers
WITH (STATE = ON);
GO





