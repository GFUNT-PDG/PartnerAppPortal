-- Diagnose partner login email matching after CRM sync
-- Run against: pacappdb01

-- 1) Account emails currently usable for login
SELECT Name, PrimaryContact, Email, SyncedOn
FROM dbo.CrmAccounts
WHERE Email IS NOT NULL AND LTRIM(RTRIM(Email)) <> ''
ORDER BY Name;

-- 2) Contact emails (primary contacts should land here)
SELECT ContactId, FirstName, LastName, Email, AccountId
FROM dbo.CrmContacts
WHERE Email IS NOT NULL AND LTRIM(RTRIM(Email)) <> ''
ORDER BY Email;

-- 3) Coverage
SELECT
  (SELECT COUNT(*) FROM dbo.CrmAccounts) AS Accounts,
  (SELECT COUNT(*) FROM dbo.CrmAccounts WHERE Email IS NOT NULL AND LTRIM(RTRIM(Email)) <> '') AS AccountsWithEmail,
  (SELECT COUNT(*) FROM dbo.CrmAccounts WHERE PrimaryContact IS NOT NULL AND LTRIM(RTRIM(PrimaryContact)) <> '') AS AccountsWithPrimaryContact,
  (SELECT COUNT(*) FROM dbo.CrmContacts) AS Contacts,
  (SELECT COUNT(*) FROM dbo.CrmContacts WHERE Email IS NOT NULL AND LTRIM(RTRIM(Email)) <> '') AS ContactsWithEmail;

-- 4) Optional test: set account email so partner login works while sync is fixed
-- UPDATE dbo.CrmAccounts
-- SET Email = N'you@partnerdomain.com',
--     PrimaryContact = N'Contact Name'
-- WHERE Name = N'Your Account Name';
-- GO
