--Run before encryption

-- Use the correct DB first
USE [sms-db-dev];
GO

-- Variables
DECLARE @SubscriberId UNIQUEIDENTIFIER = NEWID();
DECLARE @ApplicationId UNIQUEIDENTIFIER = NEWID();
DECLARE @SecretId UNIQUEIDENTIFIER = NEWID();
DECLARE @EmailId UNIQUEIDENTIFIER = NEWID();
DECLARE @PhoneId UNIQUEIDENTIFIER = NEWID();

-- Seed Subscriber
INSERT INTO [adm].[Subscribers] (SubscriberId, MicrosoftGraphOwnerId, Seeded)
VALUES (@SubscriberId, 'fake-owner-id-tenant-1234', 1);

-- Seed Application
INSERT INTO [adm].[Applications] (ApplicationId, MicrosoftGraphApiAppId, Seeded, SubscriberId)
VALUES (@ApplicationId, 'fake-api-app-id-5678', 1, @SubscriberId);

-- Seed Secret
INSERT INTO [usr].[Secrets] (
    SecretId, MicrosoftGraphApiKeyId, DisplayName, EndDateTime,
    LastTimeNotified, ContactByEmail, ContactBySMS, ContactByApiEndpoint,
    Seeded, ApplicationId, SubscriberId)
VALUES (
    @SecretId, 'fake-secret-key-id-9876', 'Test Key 1', DATEADD(month, 6, GETUTCDATE()),
    NULL, 1, 1, 0, 1, @ApplicationId, @SubscriberId
);

-- Seed Email
INSERT INTO [suprusr].[Emails] (EmailId, EmailAddress, Seeded)
VALUES (@EmailId, 'olle@example.com', 1);

-- Seed Phone
INSERT INTO [suprusr].[Phones] (PhoneId, PhoneNumber, Seeded)
VALUES (@PhoneId, '+46700000001', 1);

-- Link Email to Secret
INSERT INTO [suprusr].[EmailSecret] (EmailsEmailId, SecretsSecretId)
VALUES (@EmailId, @SecretId);

-- Link Phone to Secret
INSERT INTO [suprusr].[PhoneSecret] (PhonesPhoneId, SecretsSecretId)
VALUES (@PhoneId, @SecretId);
