
--Create schemas:
--for the regular user
CREATE SCHEMA usr;
GO

--for external administrators
CREATE SCHEMA suprusr;
GO

-- Create users mapped to Entra ID groups (replace with actual Entra group names)
CREATE USER [SecretManagement_ExternalAdministrators] FROM EXTERNAL PROVIDER;
CREATE USER [SecretManagement_Users] FROM EXTERNAL PROVIDER;
CREATE USER [SecretManagement_FunctionApp] FROM EXTERNAL PROVIDER;

--create roles
CREATE ROLE ExternalAdminRole;
CREATE ROLE UserRole;
CREATE ROLE AppRole;

--Assign Entra-based users to roles
ALTER ROLE ExternalAdminRole ADD MEMBER [SecretManagement_ExternalAdministrators];
ALTER ROLE UserRole ADD MEMBER [SecretManagement_Users];
ALTER ROLE AppRole ADD MEMBER [SecretManagement_FunctionApp];
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
CREATE FUNCTION Security.fn_subscriber_filter(@SubscriberId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS fn_subscriber_filter_result
    WHERE @SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER);
GO

CREATE FUNCTION Security.fn_secret_filter(@SecretId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN 
    SELECT 1 AS fn_secret_filter_result
    WHERE EXISTS (
        SELECT 1 
        FROM usr.Secrets s
        WHERE s.SecretId = @SecretId
          AND s.SubscriberId = CAST(SESSION_CONTEXT(N'SubscriberId') AS UNIQUEIDENTIFIER)
    );
GO




--Create and apply Security Policies for RLS:
CREATE SECURITY POLICY SubscriberSecurityPolicy
ADD FILTER PREDICATE Security.fn_subscriber_filter(SubscriberId)
    ON usr.Secrets,
ADD FILTER PREDICATE Security.fn_subscriber_filter(SubscriberId)
    ON suprusr.Applications,
ADD FILTER PREDICATE Security.fn_subscriber_filter(SubscriberId)
    ON suprusr.Subscribers,
ADD FILTER PREDICATE Security.fn_secret_filter(SecretId)
    ON suprusr.Phones,
ADD FILTER PREDICATE Security.fn_secret_filter(SecretId)
    ON suprusr.Emails,
ADD FILTER PREDICATE Security.fn_secret_filter(SecretId)
    ON suprusr.ApiEndpoints
WITH (STATE = ON);
GO




