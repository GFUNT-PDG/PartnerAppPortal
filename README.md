# Partner Assessment Application — Production v.1

Local migration baseline for standing up this application in a **new Azure subscription**.

## Contents

| Path | Purpose |
|------|---------|
| `PartnerAppPortal.sln` | Visual Studio / `dotnet` solution |
| `src/Api` | ASP.NET Core Web API |
| `src/Portal` | Blazor Server portal |
| `src/Core`, `Application`, `Infrastructure`, `Modules` | Shared libraries |
| `src/sql` | Azure SQL DDL / fix / seed scripts |
| `src/crmsync` | Power Automate CRM → SQL sync guides |
| `src/docs` | Diagrams and notes |
| `src/_excel_extract` | Excel question extract used for scoring rebuild |

## Local build

```powershell
cd C:\PartnerAppPortal\Production\v.1
dotnet restore PartnerAppPortal.sln
dotnet build PartnerAppPortal.sln -c Release
```

## Publish (deploy folders)

Always use **`publish-fix`**:

```powershell
dotnet publish src\Api\Api.csproj -c Release -o src\Api\publish-fix
dotnet publish src\Portal\Portal.csproj -c Release -o src\Portal\publish-fix
```

Zip and deploy those folders to App Service (API + Portal).

## Configuration

- Committed `appsettings.json` files use **placeholders only** (no live passwords).
- Copy `appsettings.Local.json.example` → `appsettings.Local.json` for local testing (gitignored).
- Target end-state: **Azure Key Vault** + managed identity / SPN (see `docs/MIGRATION.md`).

## SQL

See `docs/SQL-SETUP.md` for script order against the new Azure SQL database.

## Git

This folder is a **local git repo**. Add a remote when the new subscription / Git hosting is ready.
