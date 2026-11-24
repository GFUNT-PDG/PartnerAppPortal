# PDG Partner Hub — change request status

Saved 16 November 2026. Waiting on program approval. Do not start implementation until approval, and until the two hierarchy charts name the Dynamics columns.

Repository: `C:\PartnerAppPortal\Production\v.1` (new production). Do not mix these changes into App Development (`C:\PartnerAppPortal\src`) unless Gary asks to promote them.

## Request

- Add a Co-op funding tile.
- Add a Training/Certification tile.
- Employee customer dropdown shows only active customers assigned to the PDG rep who is signed in.
- A separate closed-projects list for reference and follow-up.
- Partner sign-in change for vendors who enter with their email.
- CRM sync must bring the extra fields those rules need.

## Agreed phases

Go through these in order after approval. Phases 3 and 4 can proceed together once phase 2 is writing the new fields. Phase 5 can start once the charts say where the tiles live.

### 1. CRM field contract

Gary provides two hierarchy charts with Dynamics column names. Do not guess columns.

- Employee chart: which account fields mark a customer active, which PDG user owns it, and what closed means.
- Vendor chart: which email on the account or contact is the vendor login, and whether one vendor email can open more than one customer.

### 2. Sync pipeline

Add the named columns on `dbo.CrmAccounts` (and contacts, if the vendor chart uses them). Add those fields to the Power Automate incremental upsert. Keep the watermark sync. Do not full-reload. Deploy the API only after the columns exist. The CRM flow URL and Dataverse client secret stay in API settings, not in source or git.

### 3. Employee customer scope

The employee dropdown (`/dashboard/employee`) lists only active customers assigned to the signed-in PDG account. Closed projects are a separate reference list, not a second working dropdown.

Onboarding (`/employee/onboarding`) uses the same customer list today. The employee chart decides whether onboarding stays on the full list or uses the same active-only filter.

### 4. Vendor sign-in

PDG addresses (`@pdg.com` and `@partnerdevelopmentgroup.com`) stay on the employee portal. Only the non-PDG path changes.

Today that path matches one CRM email and opens one partner customer. If the vendor chart says one email can reach several customers, the partner home must let them choose. If it stays one customer, this is a tighter match on the same sign-in page.

### 5. New tiles

Co-op funding and Training/Certification follow the Sales Accelerator pattern: a hub tile, then a page. The charts say whether each tile is employee, partner, or both, and whether a CRM flag turns it on per customer. They do not wait on the dropdown unless access is stored on the account.

## Current behavior these phases will change

- `GetCustomersAsync` returns every `dbo.CrmAccounts` row, ordered by name. The employee dropdown and onboarding both call it.
- `dbo.CrmAccounts` as mapped in the portal keeps name, email, primary contact, and `SalesAcceleratorAccess`. Account executive and development director are on the model and ignored.
- Sign-in is Microsoft Entra. PDG emails open `/dashboard/employee`. Any other email is matched to one CRM account or contact and opens `/dashboard/partner?customerId=…`.
- Company names sync from Dataverse to SQL through the incremental Power Automate flow. The employee button “Refresh customers from CRM” starts that flow. Onboarding reads and writes the Dynamics account directly. It does not store those field values in SQL.
- Hub menu groups are Employee, Partner, and Productivity. Productivity tiles are links out to Microsoft, not pages in the hub.

## Resume

On approval, start at phase 1 with the hierarchy charts. Do not change the sync, the dropdown, vendor sign-in, or the new tiles before those column names are in hand.
