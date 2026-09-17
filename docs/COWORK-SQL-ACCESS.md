# CoWork access to assessment results (Option 1)

Near-term: **read-only Azure SQL** via dedicated views + `cowork_readonly` login.  
Later (new production + Entra): switch agents to **API** (Option 2).

## 1. Create views

Run on the app database:

`sql/CreateCoWorkAssessmentViews.sql`

| View | Purpose |
|------|---------|
| `dbo.vw_CoWork_AssessmentHeaders` | Submitted assessments + overall/baseline scores + account |
| `dbo.vw_CoWork_AssessmentAnswers` | Question-level responses/points for submitted assessments |
| `dbo.vw_CoWork_LatestAssessmentByAccount` | Latest submitted assessment per CRM account |

Only rows with `SubmittedAt > 1753-01-01` (excludes in-progress).

## 2. Create read-only login/user

1. Connect to **master** → create login (see `sql/CreateCoWorkReadonlyUser.sql`).
2. Connect to **app DB** → create user, grant `db_cowork_reader` on the three views only.
3. Store the password outside git (password manager / Key Vault later).

## 3. Point CoWork at SQL

- Server / database / SQL auth as `cowork_readonly`
- Query the **views**, not base tables
- Ensure SQL firewall allows the CoWork / Azure Connectors path

## 4. Example agent queries

```sql
-- Latest scorecard per partner
SELECT * FROM dbo.vw_CoWork_LatestAssessmentByAccount
ORDER BY AccountName;

-- One partner's full answer sheet (latest)
SELECT a.*
FROM dbo.vw_CoWork_AssessmentAnswers a
INNER JOIN dbo.vw_CoWork_LatestAssessmentByAccount l
  ON l.AssessmentId = a.AssessmentId
WHERE l.AccountName = N'Test Company'
ORDER BY a.Category, a.DisplayOrder;
```

## 5. Option 2 (later)

When Entra is live on new production, give CoWork an app registration and call API endpoints (assessments by customer, answers, scores) instead of direct SQL.
