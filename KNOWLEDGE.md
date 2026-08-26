# KNOWLEDGE.md — Gotchas, Patterns & Decisions

> Accumulated know-how for working in this repo. Add an entry whenever you learn something non-obvious.
> Format: `## Topic` → facts, code refs, do/don't.

## Build & Run

- **`dotnet build` fails with MSB3027 "file locked by ExamSystem (PID)" if the app is running.** The C# compile itself succeeded; only the copy of `apphost.exe` fails. Stop the app (or `taskkill /PID <pid>`) before rebuilding. Don't mistake this for a code error.
- Solution builds with .NET SDK 10 installed while targeting `net8.0` — that's fine.

## Database (dual-provider)

- Provider switch is a **one-liner** in `appsettings.json` → `"Database": { "Provider": "MariaDb" | "SqlServer" }`. Handled in `Utilities/DatabaseHelper.cs` + `Program.cs` DbContext setup (`UseMySql` vs `UseSqlServer`).
- MariaDB uses Pomelo with `ServerVersion.AutoDetect(connectionString)` → DB must be reachable at startup or it throws.
- Schema lives as plain SQL scripts in `Database/` (000 create tables, 001 seed). EF `Migrations/` folder also exists — confirm which source of truth applies before changing schema. Currently SQL scripts are the bootstrap path.
- Entities: `m_` prefix = master data (grade, subject, question...), `t_` = transactions (exam, exam_question) — see `Entity/`.

## Auth Model (two schemes)

- Web MVC pages: cookie auth via `[AuthorizeUser]` attribute + `WebPolicy`; login page `/admin/login`; 60-min sliding session.
- API endpoints: JWT Bearer + `ApiPolicy`, attribute `[AuthorizeToken]`. JWT config from `JwtSettings` section; `ClockSkew.Zero`.
- Current-user access inside services/static contexts: static `AuthUser` / `Auth` classes configured once from `IHttpContextAccessor` in Program.cs — they are request-scoped singletons over HttpContext, not DI-scoped.
- Session service (`SessionService`, scoped) backs flash messages and login state.

## Response & Error Contract

- Services return `{ IsSuccess, Data, Meta, Errors{ Detail, InvalidParams } }`.
- Controller pattern on failure: add `InvalidParams` to ModelState (`AddModelErrors`), else flash `ErrorMessage(Detail)`; re-render form view with the submitted model. See `ExamController.Generate` for the canonical example.
- Global errors: `GlobalExceptionHandlingMiddleware` catches unhandled; `NotFoundException` is caught per-controller where a redirect+flash is nicer (see `ExamController.Delete`).

## PDF Export Pipeline

- Flow: load exam → render `Views/Exam/PrintTemplate.cshtml` to string via `ViewRenderService.RenderToStringAsync` → `IExamService.ExportToPdfAsync` (WkHtmlToPdf) → return `File(pdf, "application/pdf")`, inline disposition.
- If Razor rendering throws, controller falls back to hand-built HTML (`FallbackPrintHtml`) so export never hard-fails.
- WkHtmlToPdf converter is registered as **singleton** `SynchronizedConverter(new PdfTools())` — it's not thread-safe natively, hence synchronized wrapper. Never register transient.
- Question images in print HTML: absolute URLs required (`UploadSetting:BaseUrl` points into wwwroot/uploads).

## MVC / Razor Gotchas

- **MVC1000**: sync `Html.Partial()` used in 6 Index views → deadlock risk under load. Use `<partial name="..." />` tag helper when touching those files.
- Views are referenced by explicit path strings like `"~/Views/Exam/Index.cshtml"` instead of conventional resolution (controllers live in namespace `Controllers.Admin`). Keep this pattern when adding views.
- Form-heavy POSTs need raised limits: exam generate uses `[RequestFormLimits(ValueCountLimit = 4096)]` (global limit is 2048 in Program.cs `FormOptions`).

## Program.cs Composition Notes

- Registration order matters: `UseStatusCodePagesWithReExecute("/errors/{0}")` first; exception handler/HSTS only outside Development; middleware order = StaticFiles → Routing → Authentication → Authorization → Session → custom middlewares → MVC route `{controller=Dashboard}/{action=Index}/{id?}`.
- Known duplicates to clean someday: `DatabaseHelper.Initialize` ×2, `AddControllers()` ×2 + `AddControllersWithViews()` ×2 (second one adds localization).
- `Consts.Configure(builder.Configuration)` must run before anything reading `Constraints/Consts`.

## Conventions To Follow When Adding a Module

1. Entity in `Entity/m_{name}.cs` (+ table SQL in `Database/`)
2. DTOs in `DTOs/{Module}/`, ViewModels in `ViewModels/{Module}/`
3. `I{Name}Dao`/`{Name}Dao` in `DAO/{Module}/` (+ `BaseDao` inheritance), `I{Name}Service`/`{Name}Service` in `Services/{Module}/`
4. Register both pairs in `Program.cs` (Scoped)
5. Controller in `Controllers/Admin/`, routes under `/admin`, guard with `[AuthorizeUser]`
6. Views in `Views/{Module}/`, dropdowns via `DropDownHelper`, lists paginated via `PaginationHelper`

## Security Posture (as-is)

- Secrets currently committed in `appsettings.json` (dev-only DB passwords + JWT key). Plan: move to user secrets/env before any real deployment.
- ForwardedHeaders trusts every proxy (KnownNetworks/KnownProxies cleared) → client IPs spoofable behind misconfigured hosting.
- Passwords hashed with BCrypt.Net-Next.

## Frontend / Admin UI

- **Two layouts, keep in sync**: `Views/Shared/_AdminLayout.cshtml` (admin shell: sidebar+header+toast) and `_Layout.cshtml` (public). Shared concerns (flash-message markup, script/CSS policy) must be edited in BOTH.
- **Flash messages**: `BaseController.SuccessMessage/WarningMessage/ErrorMessage` set TempData keys `ShowToast` / `ToastTitle` / `ToastIcon` / `ToastMessage`. Layouts render a Bootstrap 4 toast (top-right, `role="status"`, auto-dismiss 4s; errors 8s via `text-danger` check on icon class). Old `ShowModal*` keys no longer exist.
- **Script/CSS policy**: global layouts load ONLY jQuery + bootstrap bundle + site/admin css+js. KaTeX + Quill are self-loaded per-page via `HeadCss`/`Scripts` sections (Question Create/Edit only — those views are self-contained; don't re-add plugins globally). DataTables and MathJax were removed entirely — they were never referenced by any view.
- **Bundling has no build task**: no BuildBundlerMinifier package. `bundleconfig.json` is documentation; the actual `wwwroot/css/bundle.min.css` / `js/bundle.min.js` are produced by manual concatenation of vendored files. If you change any input file or bundleconfig, regenerate the bundle output — prod pages reference them and will 404/bloat otherwise. Dev environment bypasses bundles (CDN + fallback).
- **Vendored assets**: `wwwroot/plugins/` was downloaded from jsdelivr at pinned versions (bootstrap 4.6.2 css+js, jquery 3.7.1, fontawesome 6.5.2 css + 8 webfonts, katex 0.16.11 css+js+auto-render, quill 1.3.7 css+js). KaTeX webfonts (~60 files) intentionally NOT vendored — math falls back to CDN when offline. To bump a version: re-download plugin file AND regenerate bundles.
- **Sidebar collapse mechanics**: toggler adds/removes body class `sidebar-collapse`; all rail rules live under `@media (min-width:768px)` in `admin.css` (width 250→70px, `.nav-link p` hidden, icons centered/enlarged, brand shows `.brand-logo` only, margins →70px with .25s transitions). State key `sessionStorage.sidebarCollapsed`, applied pre-paint by a synchronous `<script>` immediately after `<body>` open tag in _AdminLayout — do NOT move that restore into the bottom jQuery-ready block (causes expanded-menu flash on every navigation). Mobile (<768px) ignores the class entirely (media query forces translateX(-100%) unless `.sidebar-open`).
- **CSS traps**: Bootstrap reboot gives `<p>` margin-bottom 1rem → zero it for flex-aligned nav labels (`admin.css`). `site.css` sets global `a:hover { underline + purple }` → `admin.css` resets decoration for chrome elements (sidebar/navbar/dropdown/stat-footer/a.btn) while content links keep the affordance. Brand link pinned white with opacity hover cue.
- **Dashboard KPI cards**: unified `.stat-box` gradient `#5a67d8 → var(--brand-2)` in `site.css` (contrast vs white verified: 4.78:1 start, 6.34:1 end — both AA). Don't reintroduce bg-info/success/warning/danger variety; status colors are reserved for real states.

## Tooling / Environment

- Windows dev box, PowerShell. App sometimes left running between sessions — check for the file-lock issue above first.
- **Python is not installed** (only Microsoft Store stub `python`/`python3` aliases that open the Store). The ui-ux-pro-max skill's `search.py` cannot run — query the skill's data CSVs (`styles.csv`, `typography.csv`, `ux-guidelines.csv`, …) directly with Grep/Read instead.
- Git repo was initialized but had no commits yet as of 2026-08-26; `.gitignore` rewritten same day — notably un-blocking `Database/*.sql` bootstrap scripts which the old VS template's `*.sql` rule was silently excluding.
