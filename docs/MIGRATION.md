# Azure subscription migration + security hardening

Baseline for moving Partner Assessment Application to a **new Azure subscription**, then tightening credentials.

## Phase A — Stand up resources (new subscription)

Suggested resources (names are examples):

| Resource | Role |
|----------|------|
| Resource group | Container for all app assets |
| App Service (API) | Hosts `src/Api` publish-fix |
| App Service (Portal) | Hosts `src/Portal` publish-fix |
| Azure SQL Server + Database | Application data |
| Key Vault | Secrets (SQL, future API keys) |
| App Registration / SPN (optional for CI) | Deploy + Key Vault access |
| Managed Identity on App Services | Preferred runtime access to Key Vault |

### App configuration (interim)

Until Key Vault is wired:

- Set `ConnectionStrings__DefaultConnection` on the **API** App Service (slot setting / application setting).
- Set `ApiBaseUrl` on the **Portal** App Service to the new API URL (`…/api/`).
- Prefer **not** storing the SQL password in committed `appsettings.json`.

### Deploy

```powershell
dotnet publish src\Api\Api.csproj -c Release -o src\Api\publish-fix
dotnet publish src\Portal\Portal.csproj -c Release -o src\Portal\publish-fix
```

Deploy zip/folder contents of each `publish-fix` to the matching App Service. Restart after config changes.

## Phase B — Security posture (SPN + Key Vault)

Target pattern:

1. **Key Vault** stores `ConnectionStrings--DefaultConnection` (or separate `Sql--ConnectionString`).
2. **API App Service** system-assigned (or user-assigned) **managed identity**.
3. Grant that identity **Key Vault Secrets User** (RBAC) or get/list access policy.
4. App setting uses Key Vault reference, e.g.  
   `@Microsoft.KeyVault(SecretUri=https://YOUR-VAULT.vault.azure.net/secrets/SqlConnectionString/)`
5. Remove plaintext SQL password from App Service config UI once reference works.
6. For **CI/CD** (GitHub Actions / Azure DevOps): use a dedicated **SPN** with least privilege to deploy App Services only; runtime still uses managed identity for secrets.
7. Rotate the legacy SQL password that previously lived in source config; create a new SQL login or Entra auth for the app.

### Portal

Portal currently calls the API via `ApiBaseUrl` (no SQL connection in Portal config). Keep it that way; only the API needs the database secret.

### CRM / Power Automate

- Recreate or export/import flows in the new tenant/subscription as required.
- Store any connector credentials in the Power Platform environment; do not put CRM secrets in this git repo.
- Update SQL connector to the new server and firewall rules.

## Phase C — Cutover checklist

- [ ] New SQL schema scripts applied (`docs/SQL-SETUP.md`) or bacpac restored
- [ ] CRM sync verified (accounts + contact emails for partner login)
- [ ] API healthy (`/swagger` or health endpoint)
- [ ] Portal login: employee domain + partner email match
- [ ] Assessment create / revise / scores board
- [ ] Key Vault references live; plaintext secrets removed
- [ ] Old subscription resources decommissioned after soak period
- [ ] Remote Git hosting connected to this local repo

## Out of scope for this dump

- Live production passwords (stripped from committed files)
- Existing Azure resource ARM/Bicep (to be authored for the new subscription)
- Power Automate flow definitions beyond the guides in `src/crmsync`
