# STATE.md — Current Work State (GSD)

> **Get Stuff Done flow**: pick from BACKLOG → move to DOING with a small, shippable scope → verify (build + manual check) → DONE.
> Update this file at the END of every session/task. Keep entries short. Delete stale DONE items periodically.
> Last updated: 2026-08-28

## NOW / DOING

- **Manual Question Selection for Exams complete (Phases 1-5, see DONE)**. Phase 5 reproduced the sample Burmese Economics paper via the manual workflow + PDF export; a page-by-page OK/NOT-OK comparison is recorded in `SampleData/COMPARISON.md`. Remaining nice-to-haves (documented as NOT-OK): remove the "ExamSystem" page header + "1/4" page-number footer from the PDF, align exact page-break boundaries, and (optional) show mark-expressions like "(၂ x ၅)" instead of computed totals.

## NEXT (ordered)

1. **Make the initial git commit** — repo has zero commits; everything staged as `A` (including `Database/*.sql` which the old .gitignore was hiding, and PROJECT/STATE/KNOWLEDGE docs). Review `git status`, then commit
2. Move secrets out of `appsettings.json` → user secrets (`dotnet user-secrets set ...`) or env vars: DB passwords, `JwtSettings:Key` — do this BEFORE pushing anywhere public
3. Replace sync `Html.Partial(...)` with `<partial>` tag helper in 6 views: `Views/{Subject,AdminUser,Exam,Grade,MarkingRule,Question}/Index.cshtml`
4. Clean Program.cs duplicates: `DatabaseHelper.Initialize` called twice; collapse `AddControllers()`/`AddControllersWithViews()` into one chain
5. Restrict ForwardedHeaders (`KnownNetworks`/`KnownProxies`) to the actual reverse proxy
6. Upgrade/patch log4net (NU1902) or evaluate suppression rationale
7. **Exam module**: full drag-drop reorder verification (within + between sections) and PDF render of multi-section exam with Myanmar template on real multi-question data — **mostly done in Phase 5** (reproduced the 39-question sample paper PDF); remaining polish: strip the running "ExamSystem" header + footer page numbers from the PDF export, and match exact page breaks / mark-expression labels (see `SampleData/COMPARISON.md`)

## BACKLOG (unordered ideas)

- Add automated test project (none exists today)
- API-side endpoints for exam data (JWT infra exists but few/no API controllers use `[AuthorizeToken]`)
- Log rotation/cleanup strategy for `Logs/MVC-log-file/`
- Consider removing unused packages (PdfSharpCore vs ClosedXML overlap — confirm which is actually used)
- Unify accent tokens: site.css `--brand-1` (#667eea) fails AA on white link text; consider darker token or scoped usage
- Add `prefers-reduced-motion` guard for sidebar/toast transitions
- Dashboard trend charts (Executive Dashboard profile: KPI + sparklines)

## DONE (recent first)

- 2026-08-28 — **Manual Question Selection for Exams (Phase 5: sample reproduction + OK/NOT-OK record)**. Seeded the full sample Economics paper (from `SampleData/eco_exam_md.md`) into the live DB (`m_questions` ids 101–139, subject 9/Grade 10) and built **exam id 8 `EX-ECO-SAMPLE-2026` "ဘောဂဗေဒ"** (100 marks, 180 min) with Q1's (က)(ခ)(ဂ) as separate flat sections (per decision — no data-model change). Rewrote `Views/Exam/PrintTemplate.cshtml` to match the sample style (centered board header, subject + right-aligned time, sections with right-aligned `(N) မှတ်`, Myanmar-digit item numbering per section, alphabet labels for essay/calc sections, inline MC options, bordered Q2 term table + Q5 ECO tables, `[တစ်ဖက်သို့]` footers + page breaks). Because PDF uses WkHtmlToPdf (old WebKit, no flexbox), template rebuilt with **floats + tables** and a C# StringBuilder body (avoids Razor markup-in-code-block issues). Verified: `dotnet build` clean; `/admin/exam/export-pdf/8` → 200 OK **4-page A4 PDF** (52 KB); PyMuPDF text dump + page-raster OCR confirm all 7 sections, correct content/numbering/marks and bordered tables. Created `SampleData/COMPARISON.md` documenting OK vs NOT-OK. **NOT-OK** (differences): running "ExamSystem" header + "1 / 4" page-number footer (from shared `ExportPdfAsync` Header/FooterSettings), page-break boundaries vs the DOCX, "(၂ x ၅)"-style mark labels shown as computed totals, A4 vs US-Letter geometry.
- 2026-08-28 — **Manual Question Selection for Exams (Phase 4: Edit/regenerate from Preview)**. Added an "Edit Exam" button to the Preview toolbar that links to the existing pre-filled builder (`/admin/exam/edit/{id}`), completing the loop: Preview → Edit → save → redirects back to Preview. No backend changes needed (reuses `UpdateExamManualAsync` + Edit view). Verified: Preview toolbar shows "Edit Exam" → opens pre-filled builder with persisted title/marks → save returns to Preview; no console errors; `dotnet build` clean. (Removed now-complete "edit/regenerate existing exam" BACKLOG item.)
- 2026-08-28 — **Manual Question Selection for Exams (Phase 3: Edit exam questions)**. Edit page now manages questions with the same two-pane builder as Generate. Backend: added `IExamService.UpdateExamManualAsync` (validates existence/duplicates/subject-grade, deletes+recreates `t_exam_question` in exact order/marks/sections, recomputes `t_exam.total_questions`/`total_marks`, stores config JSON, updates metadata), added `POST /admin/exam/edit-questions/{id}` (`[FromBody] ManualExamGenerateRequestDto` → JSON redirect). Frontend: rewrote `Views/Exam/Edit.cshtml` — pre-populates sections from `Model.questions` (grouped by `section_name`), same question-bank drawer + SortableJS drag-drop + inline marks; Edit GET now uses `GetExamWithQuestionsAsync` and sets `ViewBag.QuestionTypes`. Verified end-to-end: pre-populated Section A rendered with ECO question → changed marks 10→20 + title → Update → preview shows "Total Marks / Pass: 20 / 5" → PDF export 200 OK `application/pdf` ~28KB. Note: `ExamSystem.exe` must be started with **working directory = project root** (wwwroot is not copied to bin output), else static CSS/JS 404.
- 2026-08-28 — **Manual Question Selection for Exams (Phase 1 + 2)**: Replaced auto-random generation UI with manual selection. Backend: added `ManualExamGenerateRequestDto`/`ManualExamSectionDto`/`ManualExamQuestionDto` + question search DTOs (`ExamDtos.cs`), `IExamService.GenerateExamManualAsync` (validates existence/duplicates/subject-grade, exact order+marks), `IQuestionDao.GetByIdsAsync`, `IExamQuestionDao.GetByExamId`/`UpdateRange`, `IQuestionService.SearchQuestionsAsync` (subject/grade/type/difficulty/text + pagination) + `GET /admin/question/search`, `POST /admin/exam/generate-manual` (`[FromBody]`). Frontend: rewrote `Views/Exam/Generate.cshtml` — two-pane build page with Sections builder, question-bank side drawer (SearchableSortableJS drag-drop reorder within/between sections, inline marks editing, real-time section/global marks totaling), generate via AJAX → redirect to Preview. Verified end-to-end (select question → add to section → generate → preview → PDF 200 OK). Note: the preview's "Unable to render table data" for ECO tables is a pre-existing bug, unrelated to this work
- 2026-08-26 — Rewrote .gitignore for .NET 8: fixed `*.sql` rule that silently excluded Database schema bootstrap scripts (now negated via `!Database/*.sql`), added root `Logs/`, `.vs/`, OS junk; removed VS-autogenerated duplicates. Verified via `git check-ignore`
- 2026-08-26 — Admin dashboard UI audit (9 fixes): a11y (`aria-current`, removed `role="menu"`, 44px touch targets), flash modal → auto-dismiss BS4 toast (TempData `ShowModal*`→`ShowToast*` in BaseController + both layouts), script diet (DataTables/KaTeX/MathJax/Quill off global layouts & bundles; Question Create/Edit self-load via sections), prod bootstrap dedupe, unified AA-verified `.stat-box` KPI gradient. Vendored missing `wwwroot/plugins/` (bootstrap/jquery/fontawesome+webfonts/katex/quill); generated real `bundle.min.css/js` (were 404s). Removed dead pushmenu/DataTable JS. Build OK
- 2026-08-26 — Sidebar rework: brand anchored true top-left (57px bar flush with header); desktop hamburger → 70px icon-rail collapse (labels hidden, centered icons, 🎓 logo only, `title` tooltips), margin transitions synced (.25s), state persisted via sessionStorage applied pre-paint by sync script after `<body>` (no expanded-menu flash on nav); nav `<p>` margins zeroed (label alignment)

## BLOCKERS / OPEN QUESTIONS

- None currently.

## Verification Checklist (run before marking anything DONE)

```powershell
dotnet build          # must pass (stop running app first if file locked)
```

Then manually smoke-test affected pages under `/admin/...`.
