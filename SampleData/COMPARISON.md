# Phase 5 — Sample Economics Paper Reproduction: OK / NOT-OK Comparison

**Goal:** Reproduce the sample Burmese Economics exam paper (`Economics_Exam_Paper.docx`
source → `eco_exam_paper1-1/1-2/2-1/2-2.png`, 4 pages) using the ExamSystem **manual**
question-selection workflow and its A4 PDF export, then record what matches vs. what differs.

**How the reproduction was built**
- Seeded the full sample content into the live DB as **39 questions** (subject_id 9 =
  Economics, grade_id 6 = Grade 10) + built a new exam (**id 8, code
  `EX-ECO-SAMPLE-2026`, title "ဘောဂဗေဒ"**, 100 marks, 180 min) via an EF-compatible data
  seeder. Q1's nested (က)(ခ)(ဂ) sub-parts are modeled as **separate flat sections**
  (per the agreed decision — no data-model change). Question IDs 101–139.
- Rewrote `Views/Exam/PrintTemplate.cshtml` to match the sample's visual style:
  centered board header, subject + right-aligned time line, centered instruction,
  sections with right-aligned `(N) မှတ်` marks, Myanmar-digit item numbering per
  section, alphabet labels for the essay/calc sections, inline MC options, bordered
  Q2 term table and Q5 ECO tables, and right-aligned `[တစ်ဖက်သို့]` footers with page breaks.
- WkHtmlToPdf (A4, 15mm margins) — **output: 4 pages (A4)**.

**Artifacts**
- Our PDF: `.playwright-mcp/ecopdf/real.pdf` (52 KB, 4 pages); rendered pages
  `.playwright-mcp/ecopdf/pages_real/page1-4.png`; extracted text `pages_real/ALL.txt`.
- Sample PNGs: `SampleData/eco_exam_paper1-1.png` … `eco_exam_paper2-2.png` (4 pages).

---

## Page-by-page: Ours vs. Sample

| Page | Ours | Sample (from DOCX/PNG analysis) | Verdict |
|------|------|--------------------------------|---------|
| 1 | Board header (၂၀၂၆ ခုနှစ် / တက္ကသိုလ်ဝင်စာမေးပွဲ / မြန်မာနိုင်ငံစာစစ်ဦးစီးဌာန); "ဘောဂဗေဒ" + "ခွင့်ပြုချိန် ၃ နာရီ"; "မေးခွန်းအားလုံးဖြေဆိုပါ။"; Section ၁။(က) True/False ၁–၁၀; `[တစ်ဖက်သို့]` | Board header; subject + time; instruction; Section (က) ၁–၁၀; turn note | **OK** (layout matches) |
| 2 | Section ၁။(ခ) MC ၁–၁၀; Section ၁။(ဂ) fill-blank ၁–၁၀; Section ၂။ term table; Section ၃။ heading; marks `(၂၀)` | Sample splits (ဂ) across pages; (ဂ) begins after (ခ), Q2/table, Q3 on later pages | **NOT-OK** (page-break boundaries differ — see note below) |
| 3 | Section ၃။ notes (က)(ခ)(ဂ)(ဃ); `[တစ်ဖက်သို့]` | Notes section on its own page with turn note | **~OK** (content matches; ours puts only the 4 notes here) |
| 4 | Section ၄။ essay (က)(ခ); Section ၅။ ECO calc (က)(ခ) with bordered tables + sub-items ၁/၂; — အဆုံး — | Section ၄ + ၅ with calc tables; — အဆုံး — | **OK** (tables render, content matches) |

---

## OK — Matches the sample

1. **4 pages, A4 portrait** — same page count as the sample's 4 PNGs.
2. **Centered 3-line board header** — နှစ် / တက္ကသိုလ်ဝင်စာမေးပွဲ / မြန်မာနိုင်ငံစာစစ်ဦးစီးဌာန.
3. **Subject + time line** — "ဘောဂဗေဒ" (left) with "ခွင့်ပြုချိန် ၃ နာရီ" (right-aligned).
4. **Centered instruction** — "မေးခွန်းအားလုံးဖြေဆိုပါ။".
5. **All 5 questions / 7 sections present** with correct Burmese content:
   Q1(က) True/False 10×1, Q1(ခ) MC 10×1, Q1(ဂ) fill-blank 10×1, Q2 definitions
   (10-term 2-col table), Q3 notes (က–ဃ), Q4 essays (က)(ခ), Q5 ECO calcs (က)(ခ).
6. **Myanmar-digit item numbering per section** (၁. ၂. … ၁၀.) for (က)(ခ)(ဂ); alphabet
   (က)(ခ)(ဂ)(ဃ) for Q3/Q4/Q5 — matches sample.
7. **MC options inline** — "(က) …(ခ) …(ဂ) …" inside the stem — matches sample.
8. **Fill-in-blank** `-------------` lines — matches.
9. **Q2 term table** — bordered 2-column table of 10 terms — matches.
10. **Q5 ECO bordered data tables** (price/quantity; output/cost) + sub-questions ၁/၂ — matches.
11. **Right-aligned `[တစ်ဖက်သို့]` turn notes** — matches.
12. **Total marks sum to 100**, all content faithful to `eco_exam_md.md`.

---

## NOT-OK — Differs from the sample

1. **"ExamSystem" header text on every page** — the sample has no running header.
   Source: shared `ExamService.ExportToPdfAsync` `HeaderSettings.Left = "ExamSystem"`.
2. **Footer page numbers "1 / 4"** — the sample has no page numbers (only the turn note).
   Source: shared `FooterSettings.Center = "[page] / [toPage]"`.
   → Remediation (if desired): make the export header/footer empty or exam-configurable.
3. **Page-break boundaries differ** from the sample's DOCX pagination. E.g. our page 2
   packs (ခ) + all (ဂ) + (၂) + (၃)-heading, whereas the sample starts (ဂ) on its own page
   and splits (ဂ) item 1 onto an earlier page. Exact pagination of a DOCX→PDF vs our output
   will not be identical.
4. **Section mark labels show computed totals**, e.g. "(၂၀) မှတ်", whereas the sample
   writes the expression "(၂ x ၅) မှတ်" / "(၁၀ x ၂) မှတ်". (Wording only; the numeric
   total is correct.)
5. **Page geometry** — original DOCX is US Letter @ 0.5in margins; our export is A4 @ 15mm
   margins (per the agreed "keep A4" decision).
6. **Q1 sub-parts rendered as separate section headings** "(၁။ (က)…," "၁။ (ခ)…," "၁။ (ဂ)…)"
   because they are stored as separate flat sections (agreed decision) rather than nested
   under one "၁။" — visually near-identical, but the "၁။" repeats on each sub-section heading.

---

## Notes / Caveats
- The `?`-style glyphs in the extracted text/pymupdf output are an **extraction/console
  artifact only**; the rendered PDF text and tables are correct (verified via PyMuPDF text
  extraction written as UTF-8 and page raster OCR).
- Sample OCR (Bengali/romanized) is unreliable for Burmese; structural layout was verified
  from both the text dump and page rasters.
- The `Database/000_Create_Tables_MariaDB.sql` + `001_Seed_Dummy_Data.sql` files remain
  STALE (singular table names) vs the live plural tables — do not rely on them.
