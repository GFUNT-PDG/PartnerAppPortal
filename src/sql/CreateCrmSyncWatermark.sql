-- Daily incremental CRM sync watermark
-- Run once against: pacappdb01

IF OBJECT_ID('dbo.CrmSyncWatermark', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CrmSyncWatermark
    (
        WatermarkKey   nvarchar(50)  NOT NULL CONSTRAINT PK_CrmSyncWatermark PRIMARY KEY,
        LastSyncUtc    datetime2(0)  NOT NULL,
        UpdatedOnUtc   datetime2(0)  NOT NULL
            CONSTRAINT DF_CrmSyncWatermark_UpdatedOnUtc DEFAULT (SYSUTCDATETIME())
    );
END
GO

-- Seed starting point (full history first run if you keep 1900)
IF NOT EXISTS (SELECT 1 FROM dbo.CrmSyncWatermark WHERE WatermarkKey = N'CrmAccounts')
BEGIN
    INSERT INTO dbo.CrmSyncWatermark (WatermarkKey, LastSyncUtc, UpdatedOnUtc)
    VALUES (N'CrmAccounts', '1900-01-01T00:00:00', SYSUTCDATETIME());
END
GO

CREATE OR ALTER PROCEDURE dbo.GetCrmSyncWatermark
    @WatermarkKey nvarchar(50) = N'CrmAccounts'
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.CrmSyncWatermark WHERE WatermarkKey = @WatermarkKey)
    BEGIN
        INSERT INTO dbo.CrmSyncWatermark (WatermarkKey, LastSyncUtc, UpdatedOnUtc)
        VALUES (@WatermarkKey, '1900-01-01T00:00:00', SYSUTCDATETIME());
    END

    SELECT
        WatermarkKey,
        CONVERT(varchar(33), LastSyncUtc, 127) + N'Z' AS LastSyncUtc
    FROM dbo.CrmSyncWatermark
    WHERE WatermarkKey = @WatermarkKey;
END
GO

CREATE OR ALTER PROCEDURE dbo.SetCrmSyncWatermark
    @WatermarkKey nvarchar(50) = N'CrmAccounts',
    @LastSyncUtc  nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Accept values with or without trailing Z
    DECLARE @raw nvarchar(50) = REPLACE(REPLACE(@LastSyncUtc, N'Z', N''), N'z', N'');
    DECLARE @ts datetime2(0) = TRY_CONVERT(datetime2(0), @raw, 127);
    IF @ts IS NULL
        SET @ts = SYSUTCDATETIME();

    MERGE dbo.CrmSyncWatermark AS t
    USING (SELECT @WatermarkKey AS WatermarkKey) AS s
       ON t.WatermarkKey = s.WatermarkKey
    WHEN MATCHED THEN
        UPDATE SET LastSyncUtc = @ts, UpdatedOnUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (WatermarkKey, LastSyncUtc, UpdatedOnUtc)
        VALUES (@WatermarkKey, @ts, SYSUTCDATETIME());

    SELECT
        WatermarkKey,
        CONVERT(varchar(33), LastSyncUtc, 127) + N'Z' AS LastSyncUtc
    FROM dbo.CrmSyncWatermark
    WHERE WatermarkKey = @WatermarkKey;
END
GO

-- Verify
SELECT
    WatermarkKey,
    CONVERT(varchar(33), LastSyncUtc, 127) + N'Z' AS LastSyncUtc
FROM dbo.CrmSyncWatermark;
GO
