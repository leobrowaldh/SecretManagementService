--USE zooefc;
--GO

-- remove spLogin
DROP PROCEDURE IF EXISTS gstusr.spLogin
GO

-- remove roles
ALTER ROLE efcGstUsr DROP MEMBER gstusrUser;
ALTER ROLE efcGstUsr DROP MEMBER usrUser;
ALTER ROLE efcGstUsr DROP MEMBER supusrUser;

ALTER ROLE efcUsr DROP MEMBER usrUser;
ALTER ROLE efcUsr DROP MEMBER supusrUser;

ALTER ROLE efcSupUsr DROP MEMBER supusrUser;

DROP ROLE IF EXISTS efcGstUsr;
DROP ROLE IF EXISTS efcUsr;
DROP ROLE IF EXISTS efcSupUsr;
GO

--drop users
DROP USER IF EXISTS  gstusrUser;
DROP USER IF EXISTS usrUser;
DROP USER IF EXISTS supusrUser;
GO

-- remove spDeleteAll
DROP PROCEDURE IF EXISTS supusr.spDeleteAll
GO

DROP VIEW IF EXISTS [gstusr].[vwInfoDb]
DROP VIEW IF EXISTS [gstusr].[vwInfoZoos]
DROP VIEW IF EXISTS [gstusr].[vwInfoAnimals]
DROP VIEW IF EXISTS [gstusr].[vwInfoEmployees]
GO

--drop tables in the right order not to get fk conflicts
DROP TABLE IF EXISTS supusr.Comments;
DROP TABLE IF EXISTS supusr.Banks;
DROP TABLE IF EXISTS supusr.Attractions;
DROP TABLE IF EXISTS supusr.Addresses;
DROP TABLE IF EXISTS dbo.__EFMigrationsHistory;
GO

-- drop schemas
DROP SCHEMA IF EXISTS gstusr;
DROP SCHEMA IF EXISTS usr;
DROP SCHEMA IF EXISTS supusr;
GO