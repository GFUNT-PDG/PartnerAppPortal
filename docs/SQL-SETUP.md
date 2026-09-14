# SQL setup (new subscription)

Run these against the **new** Azure SQL database (SSMS / Azure Data Studio / `sqlcmd`), in order.

Adjust database name and login for the new environment. Do **not** commit production passwords.

## 1. Core wizard / assessment tables

1. `src/sql/CreateWizardTables.sql`
2. `src/sql/CreateAssessmentAnswers.sql`
3. `src/sql/FixAssessmentAnswersColumns.sql` (if answers table already exists from an older shape)
4. `src/sql/CreateAssessmentScoreBaselines.sql`

## 2. Question catalog

5. `src/sql/RebuildAssessmentQuestionsFromExcel.sql`
6. `src/sql/FixMisclassifiedAnswerTypes.sql` (if needed)

## 3. CRM sync support

7. `src/sql/CreateCrmSyncWatermark.sql`
8. `src/sql/FixCrmSyncWatermarkFormat.sql` (if watermark format needs correction)

## 4. Optional diagnostics

- `src/sql/DiagnosePartnerLoginEmail.sql` — partner login / email matching checks

## CRM sync (Power Automate)

Follow:

- `src/crmsync/REBUILD-GUIDE.txt` — full / rebuild sync
- `src/crmsync/DAILY-INCREMENTAL-SYNC.txt` — incremental watermark sync
- `src/crmsync/flow-blueprint.json` — flow structure reference

Allow Azure SQL firewall access for Power Automate managed connectors (`AzureConnectors.*` region tags / IP ranges, or temporary “Allow Azure services” for bring-up).

## Data cutover options

1. **Schema only** in new subscription, then re-sync CRM + re-enter assessments, **or**
2. **Export/import** `pacappdb01` (bacpac / geo-copy) into the new SQL server, then re-point App Service connection strings (prefer Key Vault).

Document which option you choose in the new subscription runbook.
