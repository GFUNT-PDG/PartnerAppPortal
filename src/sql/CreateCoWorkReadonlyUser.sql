-- CoWork least-privilege SQL access (Option 1 — read-only)
-- Run against: Azure SQL logical server + database (pacappdb01 or current DB)
--
-- PREREQ: run CreateCoWorkAssessmentViews.sql first
--
-- IMPORTANT
-- 1) Replace REPLACE_WITH_STRONG_PASSWORD before running.
-- 2) Do not commit the real password to git.
-- 3) Do not reuse sqladmin or the API app connection string password.
-- 4) Azure SQL: create LOGIN on master, then USER in the app database.

------------------------------------------------------------
-- A) On master database (connect to master)
------------------------------------------------------------
-- CREATE LOGIN cowork_readonly WITH PASSWORD = N'REPLACE_WITH_STRONG_PASSWORD';
-- GO

------------------------------------------------------------
-- B) On the app database (pacappdb01)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'cowork_readonly')
BEGIN
    -- After login exists on the server:
    -- CREATE USER cowork_readonly FROM LOGIN cowork_readonly;
    PRINT 'Create user after login exists:
CREATE USER cowork_readonly FROM LOGIN cowork_readonly;';
END
GO

-- Prefer creating the user explicitly (uncomment after login exists):
-- CREATE USER cowork_readonly FROM LOGIN cowork_readonly;
-- GO

------------------------------------------------------------
-- C) Role: SELECT only on CoWork views (recommended)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'db_cowork_reader' AND type = 'R')
BEGIN
    CREATE ROLE db_cowork_reader;
END
GO

GRANT SELECT ON dbo.vw_CoWork_AssessmentHeaders            TO db_cowork_reader;
GRANT SELECT ON dbo.vw_CoWork_AssessmentAnswers             TO db_cowork_reader;
GRANT SELECT ON dbo.vw_CoWork_LatestAssessmentByAccount     TO db_cowork_reader;
GO

-- Optional: if CoWork needs account name lookup beyond the views
-- GRANT SELECT ON dbo.CrmAccounts TO db_cowork_reader;
-- GO

ALTER ROLE db_cowork_reader ADD MEMBER cowork_readonly;
GO

------------------------------------------------------------
-- D) Verify permissions
------------------------------------------------------------
-- EXECUTE AS USER = 'cowork_readonly';
-- SELECT TOP 5 * FROM dbo.vw_CoWork_LatestAssessmentByAccount;
-- SELECT TOP 5 * FROM dbo.vw_CoWork_AssessmentAnswers;
-- -- Should fail:
-- -- SELECT TOP 1 * FROM dbo.Assessments;
-- REVERT;
-- GO

------------------------------------------------------------
-- E) CoWork / Power Platform connector settings
------------------------------------------------------------
-- Server:   pacappsql01.database.windows.net   (or current server)
-- Database: pacappdb01
-- Auth:     SQL Server Authentication
-- User:     cowork_readonly
-- Password: (from Key Vault / password manager — not from this file)
--
-- Firewall: allow Azure services and/or AzureConnectors region IPs
--           (same pattern used for Power Automate CRM sync)
--
-- Upgrade path (Option 2): replace this SQL connector with Entra-protected
-- API endpoints when new production Entra is initialized.
GO
