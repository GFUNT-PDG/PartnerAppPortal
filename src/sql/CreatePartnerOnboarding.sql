-- One onboarding profile and activity checklist per Dynamics account.
-- ChecklistJson stores each activity as "" (not reviewed), "Yes", or "No".
-- Run this in db-pap-prod01 before using New Partner Onboarding.

IF OBJECT_ID(N'dbo.PartnerOnboarding', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerOnboarding
    (
        AccountId uniqueidentifier NOT NULL CONSTRAINT PK_PartnerOnboarding PRIMARY KEY,
        ProfileJson nvarchar(max) NOT NULL CONSTRAINT DF_PartnerOnboarding_ProfileJson DEFAULT (N'{}'),
        ChecklistJson nvarchar(max) NOT NULL CONSTRAINT DF_PartnerOnboarding_ChecklistJson DEFAULT (N'{}'),
        FolderCreated bit NOT NULL CONSTRAINT DF_PartnerOnboarding_FolderCreated DEFAULT (0),
        ContactCardBlob nvarchar(512) NULL,
        UpdatedOn datetime2 NULL,
        UpdatedBy nvarchar(256) NULL
    );
END
