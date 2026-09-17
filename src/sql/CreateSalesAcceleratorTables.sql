-- Sales Accelerator Accounts (lead generator)
-- Run against: pacappdb01
-- Safe to re-run (IF NOT EXISTS)

IF OBJECT_ID(N'dbo.SalesAcceleratorSubmissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SalesAcceleratorSubmissions
    (
        Id                 uniqueidentifier NOT NULL CONSTRAINT PK_SalesAcceleratorSubmissions PRIMARY KEY,
        PartnerCustomerId  uniqueidentifier NOT NULL,
        Partner            nvarchar(256)    NOT NULL,
        SubmittedBy        nvarchar(256)    NOT NULL,
        PartnerUploadDate  datetime2(7)     NOT NULL,
        OriginalFileName   nvarchar(260)    NOT NULL,
        ContentType        nvarchar(128)    NOT NULL CONSTRAINT DF_SaaSubmissions_ContentType DEFAULT (N'text/csv'),
        FileContent        varbinary(max)   NOT NULL,
        InsertedCount      int              NOT NULL CONSTRAINT DF_SaaSubmissions_Inserted DEFAULT (0),
        DuplicateCount     int              NOT NULL CONSTRAINT DF_SaaSubmissions_Dupes DEFAULT (0)
    );

    CREATE INDEX IX_SalesAcceleratorSubmissions_PartnerCustomerId
        ON dbo.SalesAcceleratorSubmissions(PartnerCustomerId);

    CREATE INDEX IX_SalesAcceleratorSubmissions_PartnerUploadDate
        ON dbo.SalesAcceleratorSubmissions(PartnerUploadDate DESC);
END
GO

IF OBJECT_ID(N'dbo.SalesAcceleratorAccounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SalesAcceleratorAccounts
    (
        Id                 uniqueidentifier NOT NULL CONSTRAINT PK_SalesAcceleratorAccounts PRIMARY KEY,
        SubmissionId       uniqueidentifier NOT NULL,
        PartnerCustomerId  uniqueidentifier NOT NULL,
        CompanyName        nvarchar(256)    NOT NULL,
        Domain             nvarchar(256)    NOT NULL,
        NormalizedDomain   nvarchar(256)    NOT NULL,
        AccountType        nvarchar(128)    NULL,
        SubmittedBy        nvarchar(256)    NOT NULL,
        Partner            nvarchar(256)    NOT NULL,
        PartnerUploadDate  datetime2(7)     NOT NULL,
        Tpid               nvarchar(128)    NULL,
        SentToMitch        datetime2(7)     NULL,
        DateSubmitted      datetime2(7)     NULL,
        DateVerified       datetime2(7)     NULL,
        CONSTRAINT FK_SalesAcceleratorAccounts_Submissions
            FOREIGN KEY (SubmissionId) REFERENCES dbo.SalesAcceleratorSubmissions(Id)
    );

    CREATE UNIQUE INDEX UX_SalesAcceleratorAccounts_NormalizedDomain
        ON dbo.SalesAcceleratorAccounts(NormalizedDomain);

    CREATE INDEX IX_SalesAcceleratorAccounts_PartnerCustomerId
        ON dbo.SalesAcceleratorAccounts(PartnerCustomerId);

    CREATE INDEX IX_SalesAcceleratorAccounts_SubmissionId
        ON dbo.SalesAcceleratorAccounts(SubmissionId);
END
GO
