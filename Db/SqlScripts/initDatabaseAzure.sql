--USE zooefc;
--GO

--01-create-schema.sql
--create a schema for guest users, i.e. not logged in
CREATE SCHEMA gstusr;
GO

--create a schema for logged in user
CREATE SCHEMA usr;
GO

--02-create-gstusr-view.sql
--create a view that gives overview of the database content
CREATE OR ALTER VIEW gstusr.vwInfoDb AS
    SELECT 'Guest user database overview' as Title,
    (SELECT COUNT(*) FROM supusr.Attractions WHERE Seeded = 1) as nrSeededAttractions, 
    (SELECT COUNT(*) FROM supusr.Attractions WHERE Seeded = 0) as nrUnseededAttractions,
    (SELECT COUNT(*) FROM supusr.Comments WHERE Seeded = 1) as nrSeededComments, 
    (SELECT COUNT(*) FROM supusr.Comments WHERE Seeded = 0) as nrUnseededComments,
    (SELECT COUNT(*) FROM supusr.Addresses WHERE Seeded = 1) as nrSeededAddresses, 
    (SELECT COUNT(*) FROM supusr.Addresses WHERE Seeded = 0) as nrUnseededAddresses,
    (SELECT COUNT(*) FROM supusr.Banks WHERE Seeded = 1) as nrSeededBanks, 
    (SELECT COUNT(*) FROM supusr.Banks WHERE Seeded = 0) as nrUnseededBanks
GO

CREATE OR ALTER VIEW gstusr.vwInfoAttractions AS
    SELECT a.strCategory, COUNT(*) as NrAttractions  FROM supusr.Attractions a
    GROUP BY a.strCategory WITH ROLLUP;
GO

CREATE OR ALTER VIEW gstusr.vwInfoComments AS
    SELECT a.strCategory as AttractionCategory, a.Name as AttractionName, COUNT(c.CommentId) as NrComments FROM supusr.Attractions a
    INNER JOIN supusr.Comments c ON c.AttractionDbMAttractionId = a.AttractionId
    GROUP BY a.strCategory, a.Name WITH ROLLUP;
GO

CREATE OR ALTER VIEW gstusr.vwInfoAddresses AS
    SELECT ad.strCity as CityName, COUNT(a.AttractionId) as NrAttractions FROM supusr.Attractions a
    INNER JOIN supusr.Addresses ad ON a.AddressDbMAddressId = ad.AddressId
    GROUP BY ad.strCity WITH ROLLUP;
GO

--03-create-supusr-sp.sql
CREATE OR ALTER PROC supusr.spDeleteAll
    @Seeded BIT = 1

    AS

    SET NOCOUNT ON;

    -- will delete here
    DELETE FROM supusr.Attractions WHERE Seeded = @Seeded;
    DELETE FROM supusr.Comments WHERE Seeded = @Seeded;
    DELETE FROM supusr.Addresses WHERE Seeded = @Seeded;
    DELETE FROM supusr.Banks WHERE Seeded = @Seeded;
    -- return new data status
    SELECT * FROM gstusr.vwInfoDb;

    --throw our own error
    --;THROW 999999, 'my own supusr.spDeleteAll Error directly from SQL Server', 1

    --show return code usage
    RETURN 0;  --indicating success
    --RETURN 1;  --indicating your own error code, in this case 1
GO

--04-create-users-azure.sql
--create 3 users we will late set credentials for these
DROP USER IF EXISTS  gstusrUser;
DROP USER IF EXISTS usrUser;
DROP USER IF EXISTS supusrUser;

CREATE USER gstusrUser WITH PASSWORD = N'pa$$Word1'; 
CREATE USER usrUser WITH PASSWORD = N'pa$$Word1'; 
CREATE USER supusrUser WITH PASSWORD = N'pa$$Word1'; 

ALTER ROLE db_datareader ADD MEMBER gstusrUser; 
ALTER ROLE db_datareader ADD MEMBER usrUser; 
ALTER ROLE db_datareader ADD MEMBER supusrUser; 
GO

--05-create-roles-credentials.sql
--create roles
CREATE ROLE efcGstUsr;
CREATE ROLE efcUsr;
CREATE ROLE efcSupUsr;

--assign securables creadentials to the roles
GRANT SELECT, EXECUTE ON SCHEMA::gstusr to efcGstUsr;
GRANT SELECT ON SCHEMA::supusr to efcUsr;
GRANT SELECT, UPDATE, INSERT, DELETE, EXECUTE ON SCHEMA::supusr to efcSupUsr;

--finally, add the users to the roles
ALTER ROLE efcGstUsr ADD MEMBER gstusrUser;

ALTER ROLE efcGstUsr ADD MEMBER usrUser;
ALTER ROLE efcUsr ADD MEMBER usrUser;

ALTER ROLE efcGstUsr ADD MEMBER supusrUser;
ALTER ROLE efcUsr ADD MEMBER supusrUser;
ALTER ROLE efcSupUsr ADD MEMBER supusrUser;
GO

--07-create-gstusr-login.sql
CREATE OR ALTER PROC gstusr.spLogin
    @UserNameOrEmail NVARCHAR(100),
    @Password NVARCHAR(200),

    @UserId UNIQUEIDENTIFIER OUTPUT,
    @UserName NVARCHAR(100) OUTPUT,
    @Role NVARCHAR(100) OUTPUT
    
    AS

    SET NOCOUNT ON;
    
    SET @UserId = NULL;
    SET @UserName = NULL;
    SET @Role = NULL;
    
    SELECT Top 1 @UserId = UserId, @UserName = UserName, @Role = [Role] FROM dbo.Users 
    WHERE ((UserName = @UserNameOrEmail) OR
           (Email IS NOT NULL AND (Email = @UserNameOrEmail))) AND ([Password] = @Password);
    
    IF (@UserId IS NULL)
    BEGIN
        ;THROW 999999, 'Login error: wrong user or password', 1
    END

GO