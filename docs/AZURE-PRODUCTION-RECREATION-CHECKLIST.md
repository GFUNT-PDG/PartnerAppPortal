# New Azure Production — Recreation & Migration Checklist

**Source of truth for this cutover:** App Development codebase (Partner App Board, assessment charts, CoWork SQL views, etc.)  
**Target:** New Azure Production subscription  
**GitHub:** [https://github.com/GFUNT-PDG/PartnerAppPortal.git](https://github.com/GFUNT-PDG/PartnerAppPortal.git)  
**Major change:** Integrate **Microsoft Entra ID** across Portal, API, and (later) CoWork/Teams

Document date: 17 September 2026

---

## 0. Goals

1. Recreate all Azure resources in the **new subscription**
2. Deploy current application (Portal + API + SQL scripts + CRM sync pattern)
3. Replace email-only gate with **Entra authentication** (employees + partners)
4. Keep Option 1 CoWork SQL read-only for now; plan Option 2 (API + Entra) after identity is live
5. Use Key Vault for secrets (no passwords in git / appsettings)

---

## 1. Code & repo

- [x] Confirm GitHub repo `GFUNT-PDG/PartnerAppPortal` receives full solution (solution + `src/`)
- [x] Confirm committed configs use **placeholders only** (no live SQL passwords)
- [x] Clone/pull on build machine from GitHub for production deploys going forward
- [x] Keep **App Development** (`C:\PartnerAppPortal\src`) as feature branch/work area until cutover; promote via GitHub

**Publish folders (unchanged convention):**

```powershell
dotnet publish src\Api\Api.csproj -c Release -o src\Api\publish-fix
dotnet publish src\Portal\Portal.csproj -c Release -o src\Portal\publish-fix
```

---

## 2. Azure resource recreation (new subscription)

Create (names are examples — align with your naming standard):


| #   | Resource                        | Purpose                                     |
| --- | ------------------------------- | ------------------------------------------- |
| 1   | Resource group                  | Container for PAC prod                      |
| 2   | Azure SQL Server + Database     | App data (assessments, CRM sync tables)     |
| 3   | App Service Plan                | Hosting                                     |
| 4   | App Service — **API**           | `publish-fix` for Api                       |
| 5   | App Service — **Portal**        | `publish-fix` for Portal                    |
| 6   | Key Vault                       | SQL connection string, future Graph secrets |
| 7   | App Registration(s) — Entra     | Portal + API auth (see §4)                  |
| 8   | (Optional) Application Insights | Logging/telemetry                           |
| 9   | (Optional) Storage              | Future document staging                     |


**Networking / firewall**

- [x] SQL firewall: App Service outbound / allow Azure services as needed
- [ ] SQL firewall: Power Automate / AzureConnectors region IPs (CRM sync)
- [ ] SQL firewall: CoWork connector path (if still using Option 1 SQL)

---

## 3. SQL database recreation

Run scripts from `src/sql/` on the **new** database (order matters):

### Core app

1. [x ] `CreateWizardTables.sql`
2. [x ] `CreateAssessmentAnswers.sql`
3. [x ] `FixAssessmentAnswersColumns.sql` (if needed)
4. [x ] `CreateAssessmentScoreBaselines.sql`
5. [x ] `RebuildAssessmentQuestionsFromExcel.sql`
6. [ x] `FixMisclassifiedAnswerTypes.sql` (if needed)

### CRM sync support

1. [x ] `CreateCrmSyncWatermark.sql`
2. [x ] `FixCrmSyncWatermarkFormat.sql` (if needed)
3. [x ] Ensure `UpsertCrmAccount` (and related) procs/tables exist per `crmsync` guides — recreate from current prod if not in repo

### CoWork Option 1

1. [ ] `CreateCoWorkAssessmentViews.sql`
2. [ ] Contained user or login `cowork_readonly` + `SELECT` on views only (`CreateCoWorkReadonlyUser.sql` / Query Editor contained-user approach)
3. [ ] Point CoWork SQL connector at new server/DB/user

### Data cutover choice

- [ ] **A)** Schema only + CRM re-sync + new assessments, **or**
- [x] **B)** Bacpac / copy from old `pacappdb01` then re-point apps

Also create any missing tables present in EF today but not in scripts if discovered at runtime (e.g. Sales Accelerator tables if used).

---

## 4. Entra Identity integration (biggest change)

### 4.1 App registrations

- [x] **Portal** app registration (SPA / Web — Blazor Server typically “Web”)
  - Redirect URIs: `https://<portal-app>.azurewebsites.net/signin-oidc` (and signout)
  - Enable ID tokens as required
- [x] **API** app registration
  - Expose an API scope, e.g. `access_as_user`
  - App roles optional (Employee vs Partner) later
- [x] Grant Portal permission to call API scope
- [x] Admin consent in the Entra tenant

### 4.2 Who signs in


| Audience      | Entra approach                                                                                          |
| ------------- | ------------------------------------------------------------------------------------------------------- |
| PDG employees | Member users in home tenant (replace `@pdg.com` / `@partnerdevelopmentgroup.com` string check)          |
| Partners      | **Entra External ID** guests / CIAM, or B2B guests already on partner Teams — must map to `CrmAccounts` |


- [ ] Define claim used to bind partner → CRM account (email / UPN)
- [ ] Keep CRM email sync accurate (Power Automate) so mapping works
- [ ] Decide: block public consumer domains still applies at CRM match layer

### 4.3 Application code changes (implement during prod build-out)

- [x] Add Microsoft.Identity.Web (or equivalent) to **Portal** and **API**
- [x] Portal: `[Authorize]` / OpenID Connect; remove or demote email-only gate to post-login CRM binding
- [x] API: JWT bearer validation; protect assessment/CRM endpoints
- [ ] HttpClient from Portal → API sends bearer token
- [ ] Config via App Settings / Key Vault: `AzureAd:TenantId`, `ClientId`, `Audience`, etc.
- [ ] Local dev: user secrets / `appsettings.Development.json` (gitignored)

### 4.4 Teams / Graph (after Entra works)

- [ ] Store `TeamsGroupId` / `ChannelId` per account (don’t rely on name alone at runtime)
- [ ] Delegated Graph for chat (30-day) + Files/Shared
- [ ] Replace `/partner/teams` and `/partner/documents` placeholders

---

## 5. Key Vault & configuration

Vault name to create: `kv-pap-prod01` (East US, same resource group as the App Services). Use **Azure role-based access control**.

Secrets to create (values copied from the current App Service settings, not from git):


| Secret name           | App Service setting                    | App    |
| --------------------- | -------------------------------------- | ------ |
| `SqlConnectionString` | `ConnectionStrings__DefaultConnection` | API    |
| `PortalClientSecret`  | `AzureAd__ClientSecret`                | Portal |


Reference value (no version in the URI, so the latest secret is used):

```text
@Microsoft.KeyVault(SecretUri=https://kv-pap-prod01.vault.azure.net/secrets/SqlConnectionString/)
@Microsoft.KeyVault(SecretUri=https://kv-pap-prod01.vault.azure.net/secrets/PortalClientSecret/)
```

- [x] Create the vault
- [x] Turn on system-assigned managed identity for the API app and the Portal app
- [x] Grant both identities **Key Vault Secrets User**
- [x] Grant your user **Key Vault Secrets Officer** and save the two secrets
- [x] Replace the two plaintext App Service settings with the references above
- [ ] Confirm each reference shows a green resolved check, then restart both apps
- [ ] Portal setting `ApiBaseUrl` stays a normal URL (no secret)
- [ ] `Graph__ResourceTenantId` stays the PDG tenant id (needed for Teams and for the portal’s app token)
- [ ] After the vault works, rotate the SQL password and update only the Key Vault secret

### API lock (after the vault, before relying on it)

Partner sign-in must not request a PAP-Api token. That fails for customer tenants (`AADSTS650052`). The portal instead calls the API as itself when `DownstreamApi__AppScope` is set to:

```text
api://bf9bc379-e1e8-4305-abb4-d384448b1c24/.default
```

- [x] On PAP-Api, add an application permission (app role) such as `access_as_app`
- [x] Grant that application permission to PA-Portal and admin-consent it
- [ ] Set `DownstreamApi__AppScope` on the Portal app, then deploy the portal build that sends the app token
- [x] Confirm `GET /api/crm/customers` without a token returns 401, and the employee dashboard still loads

Leave `DownstreamApi__AppScope` empty until that Entra permission exists. An empty value keeps today’s calls working.

---

## 6. App Service deploy

- [x] Deploy API `publish-fix` → new API App Service
- [x] Deploy Portal `publish-fix` → new Portal App Service
- [x] Verify HTTPS, health/ping, Swagger (if enabled in non-prod slot only)
- [ ] Smoke: Entra login → employee dashboard → start/submit assessment
- [ ] Smoke: Entra partner login → App Board → Assessment Portal / charts / designations

---

## 7. CRM sync (Power Automate)

- [x] Recreate or clone flows in correct environment for new subscription/tenant
- [x] Point SQL actions at **new** Azure SQL
- [x] Re-apply watermark pattern (`Get`/`SetCrmSyncWatermark` — no 1900 reset each run)
- [x] Validate partner email fields land in `CrmAccounts` / `CrmContacts`
- [x] Firewall for connectors

Guides: `src/crmsync/REBUILD-GUIDE.txt`, `DAILY-INCREMENTAL-SYNC.txt`

---

## 8. Feature inventory to verify after deploy

### Live today (must work)

- [ ] Employee assessment wizard (9 steps) + scoring (section max 100/100/80/65/50)
- [ ] Baselines / onboarding vs current
- [ ] Partner App Board (`/dashboard/partner`)
- [ ] `/partner/assessment` + section & overall charts (baseline + 90% goal)
- [ ] `/partner/designations` hot list
- [ ] CRM customer list / email resolve (via sync)

### Placeholders (deploy UI; wire later)

- [ ] Action Plans, Progress Tracker, Teams chat, Document Library, specialization detail copy

### CoWork

- [ ] Option 1 views + readonly user on **new** SQL
- [ ] Plan Option 2: API scopes for agents after Entra

---

## 9. Cutover sequence (recommended)

1. GitHub code pushed (this task)
2. Create RG + SQL + App Services + Key Vault
3. Run SQL scripts; optional bacpac
4. Deploy API + Portal with placeholder config
5. Stand up Entra app registrations; wire Portal + API auth
6. Move secrets to Key Vault; remove plaintext
7. Point CRM sync at new SQL; verify login mapping
8. CoWork readonly user on new SQL
9. Soft launch / parallel run vs old subscription
10. DNS/bookmarks cutover; decommission old apps after soak

---

## 10. Out of scope for day-1 recreate (track separately)

- Full Copilot / CoWork recommendation productization
- Dataverse as sole system of record (hybrid later)
- Production CI/CD pipeline (GitHub Actions) — recommended soon after first manual deploy

---

## 11. Rollback

- Keep old subscription App Services read-only during soak
- DNS/bookmarks can revert to old Portal URL
- SQL: retain old DB until CoWork + partners validated on new

---

*Use this checklist as the production stand-up runbook. Update checkboxes as each item completes in the new subscription.*