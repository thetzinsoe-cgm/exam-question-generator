# PROJECT.md — Exam System

> Read this first in every session. Last updated: 2026-08-26

## What This Is

**Exam Management System** — an ASP.NET Core 8 MVC admin web app for building and managing exam papers:

- Manage **Grades**, **Subjects**, a **Question Bank** (with answers, images, ECO tables), and **Marking Rules**
- **Generate exams** by selecting grade/subject → build sections → **manually pick questions from the bank** into each section (side drawer search/filter), set per-question marks, drag-drop reorder within/between sections → real-time totals. The **Edit** page reuses the same two-pane builder to manage an exam's questions (reloads `t_exam_question`, add/remove/reorder/marks)
- **Preview** an exam, then **export it as PDF** (WkHtmlToPdf) with an HTML fallback renderer
- Admin login (cookie session) + JWT tokens for API clients
- Dashboard with stats

It is NOT a student-facing online test-taking app.

## Stack

| Layer | Tech |
|---|---|
| Framework | ASP.NET Core 8.0 MVC (`Microsoft.NET.Sdk.Web`) |
| Data access | EF Core 8 + Dapper, lazy-loading proxies |
| Database | MariaDB (Pomelo) — switchable to SQL Server via `Database:Provider` |
| Auth | Cookie auth (web, `/admin/login`, WebPolicy) + JWT Bearer (API, ApiPolicy) |
| Validation | FluentValidation (+ DataAnnotations on DTOs/ViewModels) |
| PDF / Excel | WkHtmlToPdfDotNet (SynchronizedConverter singleton), ClosedXML, PdfSharpCore, SkiaSharp |
| Logging | log4net → `Logs/MVC-log-file/` |
| Serialization | Newtonsoft.Json (`MvcNewtonsoftJsonOptions`) |

## Run / Build

```powershell
dotnet build          # fails ONLY if ExamSystem.exe is already running (file lock)
dotnet run            # app starts at configured port
```

- DB connection strings & provider: `appsettings.json` → `ConnectionStrings`, `Database:Provider`
- DB schema bootstrap SQL: `Database/000_Create_Tables_MariaDB.sql`, `Database/001_Seed_Dummy_Data.sql`
- Logs land in `./Logs/`

## Architecture

Strict layering — request flows top to bottom. Business logic NEVER lives in controllers or DAOs.

```
Controllers (HTTP only: validate, call service, flash messages, redirect)
   ↓
Services (interface I{X}Service + impl {X}Service — business rules)
   ↓
DAOs (interface I{X}Dao + impl {X}Dao; BaseDao for shared query helpers; UnitOfWork wraps transaction/save)
   ↓
EF Core DbContext (Entity/exam_system_entities.cs) + Dapper raw SQL where used
```

### Module map (each module = Controller + Service + Dao + DTOs + ViewModels)

| Module | Route prefix | Purpose |
|---|---|---|
| Auth | `admin/login` | Login/logout, session, JWT issue |
| Dashboard | `dashboard` | Stats overview |
| Grade | grade CRUD | Class levels |
| Subject | subject CRUD | Subjects per grade |
| Question | question CRUD | Question bank + answer options + images |
| MarkingRule | marking-rules CRUD | Scoring config per subject |
| Exam | `admin/exam/*` | Generate / list / preview / delete / export-pdf |
| AdminUser | admin users CRUD | Manage admin accounts |
| Errors | `errors/{code}` | Status code pages (403 etc.) |

### Cross-cutting pieces

- `Program.cs` — composition root: DI registrations, auth policies, middleware order
- `Attributes/` — `[AuthorizeUser]` (web/session), `[AuthorizeToken]` (API/JWT), `[Trimmed]` model trimming
- `Middlewares/` — `GlobalExceptionHandlingMiddleware`, `PreventBackHistoryMiddleware`
- `Constraints/` — static consts: `Consts` (config-backed), `Keys`, `UserRoles`, `QuestionTypes`, `Auth`, `AuthUser` (current-user accessor)
- `Utilities/DatabaseHelper.cs` — reads provider + connection string (static, initialized once in Program)
- `Helpers/ErrorsHelper.cs` — standard API response error shapes
- `Services/PdfRender/ViewRenderService` — renders Razor views to string (for print template)

### Standard response contract (services)

Services return a common response object: `{ IsSuccess, Data, Meta, Errors { Detail, InvalidParams } }`.
Controllers map errors → `ModelState` via `AddModelErrors` / flash messages.

## Conventions

- Naming: entities `m_` (masters) / `t_` (transactions); interfaces `I{X}Service` / `I{X}Dao`
- Views live in `Views/{Module}/`; controller routes explicitly set (`[Route("admin")]`, `[HttpGet("exam/index")]`)
- Flash messages: `SuccessMessage(...)` / `ErrorMessage(...)` from `BaseController` + session
- Dropdowns built via `DropDownHelper`, pagination via `PaginationHelper.Paginated(...)`
- ViewBags for form dropdowns loaded through private `LoadViewBags()` per controller
- No comments in code unless asked; no emojis
- Nullable is DISABLED project-wide

## Known Debt / Watch List (details in STATE.md)

- Secrets (DB passwords, JWT key) committed in `appsettings.json` despite UserSecretsId
- Sync `Html.Partial()` in 6 Index views (MVC1000 deadlock warning)
- log4net 3.1.0 NU1902 moderate vulnerability
- ForwardedHeaders trusts ALL proxies (KnownNetworks/Proxies cleared)
- Duplicate registrations in Program.cs (`DatabaseHelper.Initialize` ×2, `AddControllers*` ×3)

## Doc Map

- `STATE.md` — what's done / in progress / next. **Update after every task.**
- `KNOWLEDGE.md` — gotchas, patterns, decisions, how-tos learned while developing.
