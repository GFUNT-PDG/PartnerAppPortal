-- Add Sales Accelerator Access flag to CRM accounts (partner tile gate)
-- CRM: display "Sales Accelerator Access"
-- schema name: cr3f8_SalesAcceleratorAccess
-- logical name (List rows / OData): cr3f8_salesacceleratoraccess
-- Run against: pacappdb01
-- Safe to re-run

IF COL_LENGTH(N'dbo.CrmAccounts', N'SalesAcceleratorAccess') IS NULL
BEGIN
    ALTER TABLE dbo.CrmAccounts
        ADD SalesAcceleratorAccess bit NOT NULL
            CONSTRAINT DF_CrmAccounts_SalesAcceleratorAccess DEFAULT (0);
END
GO

CREATE OR ALTER PROCEDURE dbo.SetCrmAccountSalesAcceleratorAccess
    @AccountId uniqueidentifier,
    @SalesAcceleratorAccess nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @granted bit =
        CASE
            WHEN LOWER(LTRIM(RTRIM(ISNULL(@SalesAcceleratorAccess, N'')))) IN (N'yes', N'true', N'1')
                THEN 1
            ELSE 0
        END;

    UPDATE dbo.CrmAccounts
    SET SalesAcceleratorAccess = @granted
    WHERE AccountId = @AccountId;
END
GO
