-- Step 1: Drop Security Policy first (since it depends on functions and tables)
DROP SECURITY POLICY IF EXISTS SubscriberSecurityPolicy;
GO

-- Step 2: Drop functions used by RLS
DROP FUNCTION IF EXISTS rls.fn_subscriber_filter;
DROP FUNCTION IF EXISTS rls.fn_application_filter;
DROP FUNCTION IF EXISTS rls.fn_user_subscriber_filter;
GO

-- Step 3: Drop tables in correct order
DROP TABLE IF EXISTS suprusr.EmailApplication;
DROP TABLE IF EXISTS suprusr.PhoneApplication;
DROP TABLE IF EXISTS suprusr.SubscriberUser;
DROP TABLE IF EXISTS suprusr.Phones;
DROP TABLE IF EXISTS suprusr.Emails;
DROP TABLE IF EXISTS suprusr.Users;
DROP TABLE IF EXISTS usr.Secrets;
DROP TABLE IF EXISTS suprusr.ApiEndpoints;
DROP TABLE IF EXISTS adm.Applications;
DROP TABLE IF EXISTS adm.Subscribers;
DROP TABLE IF EXISTS dbo.__EFMigrationsHistory;
GO

-- Step 4: Drop roles after removing all members
-- Revoke impersonation permissions before dropping members
REVOKE IMPERSONATE ON USER::[AppScopedUser_BackgroundTasks] FROM [func-SecretManagementService-development-001];
REVOKE IMPERSONATE ON USER::[AppScopedUser_User] FROM [func-SecretManagementService-development-001];
REVOKE IMPERSONATE ON USER::[AppScopedUser_ExternalAdmin] FROM [func-SecretManagementService-development-001];
REVOKE IMPERSONATE ON USER::[AppScopedUser_InternalAdmin] FROM [func-SecretManagementService-development-001];
REVOKE IMPERSONATE ON USER::[AppScopedUser_BackgroundTasks] FROM [leo.browaldh@innofactor.com];

-- Drop role members
ALTER ROLE NotificationFunctionAppRole DROP MEMBER [AppScopedUser_BackgroundTasks];
ALTER ROLE UserRole DROP MEMBER [AppScopedUser_User];
ALTER ROLE ExternalAdminRole DROP MEMBER [AppScopedUser_ExternalAdmin];
ALTER ROLE InternalAdminRole DROP MEMBER [AppScopedUser_InternalAdmin];

-- Drop roles
DROP ROLE IF EXISTS InternalAdminRole;
DROP ROLE IF EXISTS ExternalAdminRole;
DROP ROLE IF EXISTS UserRole;
DROP ROLE IF EXISTS NotificationFunctionAppRole;
GO

-- Step 5: Drop users
-- Revoke encryption key/view permissions
REVOKE VIEW ANY COLUMN MASTER KEY DEFINITION FROM [func-SecretManagementService-development-001];
REVOKE VIEW ANY COLUMN ENCRYPTION KEY DEFINITION FROM [func-SecretManagementService-development-001];

REVOKE VIEW ANY COLUMN MASTER KEY DEFINITION FROM [AppScopedUser_BackgroundTasks];
REVOKE VIEW ANY COLUMN ENCRYPTION KEY DEFINITION FROM [AppScopedUser_BackgroundTasks];

REVOKE VIEW ANY COLUMN MASTER KEY DEFINITION FROM [AppScopedUser_User];
REVOKE VIEW ANY COLUMN ENCRYPTION KEY DEFINITION FROM [AppScopedUser_User];

REVOKE VIEW ANY COLUMN MASTER KEY DEFINITION FROM [AppScopedUser_ExternalAdmin];
REVOKE VIEW ANY COLUMN ENCRYPTION KEY DEFINITION FROM [AppScopedUser_ExternalAdmin];

REVOKE VIEW ANY COLUMN MASTER KEY DEFINITION FROM [AppScopedUser_InternalAdmin];
REVOKE VIEW ANY COLUMN ENCRYPTION KEY DEFINITION FROM [AppScopedUser_InternalAdmin];

REVOKE VIEW ANY COLUMN MASTER KEY DEFINITION FROM [leo.browaldh@innofactor.com];
REVOKE VIEW ANY COLUMN ENCRYPTION KEY DEFINITION FROM [leo.browaldh@innofactor.com];

-- Drop users
DROP USER IF EXISTS [func-SecretManagementService-development-001];
DROP USER IF EXISTS [AppScopedUser_BackgroundTasks];
DROP USER IF EXISTS [AppScopedUser_User];
DROP USER IF EXISTS [AppScopedUser_ExternalAdmin];
DROP USER IF EXISTS [AppScopedUser_InternalAdmin];
DROP USER IF EXISTS [leo.browaldh@innofactor.com];
GO

-- Step 6: Drop schemas last
DROP SCHEMA IF EXISTS rls;
DROP SCHEMA IF EXISTS usr;
DROP SCHEMA IF EXISTS suprusr;
DROP SCHEMA IF EXISTS adm;
GO
