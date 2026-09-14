-- Wizard section tables required by Partner App Portal
-- Run against: pacappdb01
-- Safe to re-run (IF NOT EXISTS)

IF OBJECT_ID(N'dbo.PartnerInfo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PartnerInfo
    (
        Id            uniqueidentifier NOT NULL CONSTRAINT PK_PartnerInfo PRIMARY KEY,
        AssessmentId  uniqueidentifier NOT NULL,
        PartnerName   nvarchar(200)    NULL,
        MpnId         nvarchar(100)    NULL,
        Country       nvarchar(100)    NULL,
        Segment       nvarchar(100)    NULL
    );
    CREATE INDEX IX_PartnerInfo_AssessmentId ON dbo.PartnerInfo(AssessmentId);
END
GO

IF OBJECT_ID(N'dbo.CustomersCsp', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CustomersCsp
    (
        Id                   uniqueidentifier NOT NULL CONSTRAINT PK_CustomersCsp PRIMARY KEY,
        AssessmentId         uniqueidentifier NOT NULL,
        CspTier              nvarchar(100)    NULL,
        ActiveSubscriptions  int              NULL,
        AzureConsumption     decimal(18,2)    NULL,
        MarketplaceUsage     decimal(18,2)    NULL,
        IncentivesUsage      decimal(18,2)    NULL
    );
    CREATE INDEX IX_CustomersCsp_AssessmentId ON dbo.CustomersCsp(AssessmentId);
END
GO

IF OBJECT_ID(N'dbo.CoSellReferrals', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoSellReferrals
    (
        Id                   uniqueidentifier NOT NULL CONSTRAINT PK_CoSellReferrals PRIMARY KEY,
        AssessmentId         uniqueidentifier NOT NULL,
        CoSellOpportunities  int              NULL,
        ReferralsSubmitted   int              NULL,
        ReferralsWon         int              NULL,
        PipelineValue        decimal(18,2)    NULL
    );
    CREATE INDEX IX_CoSellReferrals_AssessmentId ON dbo.CoSellReferrals(AssessmentId);
END
GO

IF OBJECT_ID(N'dbo.MarketplaceUsage', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MarketplaceUsage
    (
        Id            uniqueidentifier NOT NULL CONSTRAINT PK_MarketplaceUsage PRIMARY KEY,
        AssessmentId  uniqueidentifier NOT NULL,
        Offers        int              NULL,
        Transactions  int              NULL,
        Revenue       decimal(18,2)    NULL,
        Growth        decimal(18,2)    NULL
    );
    CREATE INDEX IX_MarketplaceUsage_AssessmentId ON dbo.MarketplaceUsage(AssessmentId);
END
GO

IF OBJECT_ID(N'dbo.IncentivesUsage', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IncentivesUsage
    (
        Id            uniqueidentifier NOT NULL CONSTRAINT PK_IncentivesUsage PRIMARY KEY,
        AssessmentId  uniqueidentifier NOT NULL,
        Earned        decimal(18,2)    NULL,
        Redeemed      decimal(18,2)    NULL,
        Balance       decimal(18,2)    NULL,
        Growth        decimal(18,2)    NULL
    );
    CREATE INDEX IX_IncentivesUsage_AssessmentId ON dbo.IncentivesUsage(AssessmentId);
END
GO

IF OBJECT_ID(N'dbo.GrowthMetrics', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GrowthMetrics
    (
        Id              uniqueidentifier NOT NULL CONSTRAINT PK_GrowthMetrics PRIMARY KEY,
        AssessmentId    uniqueidentifier NOT NULL,
        YearOverYear    decimal(18,2)    NULL,
        CustomerAdds    int              NULL,
        RevenueGrowth   decimal(18,2)    NULL,
        PipelineGrowth  decimal(18,2)    NULL
    );
    CREATE INDEX IX_GrowthMetrics_AssessmentId ON dbo.GrowthMetrics(AssessmentId);
END
GO
