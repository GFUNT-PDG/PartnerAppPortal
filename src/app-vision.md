# Application Vision & Functional Parameters
**Target Stack:** .NET 8.0 Web API, Azure SQL Database, Azure Web App Services
**Core Purpose:** Web-facing Microsoft Partnership Assessment & Onboarding Tool

## 1. Core System & Business Vision
* **Primary Objective:** Provide a seamless, public-facing portal where potential customers can self-assess their current Microsoft partnership status and complete onboarding workflows.
* **Data Context:** The application gathers user inputs, saves assessment matrices, and moves users through a structured onboarding pipeline. There are currently no complex row-level data restrictions required between customers.
* **Modular Architecture:** The system must strictly enforce separation of concerns. The assessment scoring logic, onboarding state machine, and portal APIs must live in isolated, clean modules.

## 2. Authentication & Security Parameters
* **Frontend Authentication (Microsoft Entra ID / Azure AD):** Users must authenticate against Azure AD using modern OpenID Connect / JWT token verification. No local passwords or user identity tables should exist on the application level.
* **Backend Database Authentication (SQL Auth):** The .NET 8 App Service communicates with Azure SQL exclusively via standard SQL Server Authentication using the SqlAdmin database service account credentials. 
* **Token Isolation:** The application must cleanly map the Azure AD claim identity to custom assessment session tables without letting backend SQL queries bleed or require Entra ID mapping on the database plane.

## 3. Extension & Integration Roadmap
* **Future Plug-and-Play Modules:** The backend architecture must allow 3 additional assessment or processing modules to be introduced later without changing core routing, breaking the base user tracking, or altering existing data schemas.
* **External Portals & Power Apps Integration:** The modular API backend must expose dedicated, clean, secured REST endpoints. These endpoints will serve as data pipelines to feed onboarding data seamlessly into 3 future external Power Apps dashboards.

## 4. Technical Quality & Azure Performance Guardrails
* **Async/Await Hygiene:** Because this tool handles heavy I/O operations (reading Azure SQL records while processing multi-step customer assessments), all database and network operations must be natively asynchronous down to the data access layer.
* **EF Core Integrity:** Ensure Entity Framework Core manages the `Saladin` user connections optimally using modern pooling, avoiding thread starvation, and applying `.AsNoTracking()` to assessment lookups to save Azure SQL compute cycles.
* **Error Resilience:** System errors must be handled gracefully by global exception middleware. Under no circumstances should a raw Azure SQL connectivity or execution error be leaked to the customer-facing frontend.