# ECMA-55 Specification Corpus

This directory converts `docs/ECMA-55_1st_edition_january_1978.pdf` into a repository-local specification set that can be referenced by tests, issues, and implementation work.

## Contents

- `requirements.md`: Derived, testable requirements with stable IDs.
- `traceability.md`: Section-to-requirement mapping back to ECMA-55 pages.
- `source/ecma-55-ocr.txt`: Working OCR corpus generated from the PDF for local traceability and review.

## Goal

The original ECMA-55 document is written as a language standard, not as a modern engineering specification. The files in this directory restate its normative content as independently verifiable requirements so that we can:

- reference exact requirement IDs in tests and GitHub issues,
- separate parser, runtime, and documentation obligations,
- identify implementation-defined behavior that must be documented, and
- perform coverage and gap analysis against the current interpreter.

## Requirement Format

Each requirement uses this shape:

- `ID`: Stable identifier, for example `ECMA55-PRG-004`.
- `Requirement`: A paraphrased, testable statement derived from the standard.
- `Verification`: The primary verification mode.
- `Source`: The ECMA-55 section and PDF page range.

## Verification Modes

- `Parser`: Verified by parsing or rejecting source text.
- `Runtime`: Verified by executing a BASIC program and checking behavior.
- `Documentation`: Verified by checking implementation documentation or configuration docs.
- `Manual`: Verified by targeted human review when automation is impractical.

## Important Notes

- This corpus is derived from the local PDF plus a repository-local OCR pass. It should be treated as a working specification set, not as a replacement for the original ECMA text.
- Table-heavy pages and a few syntax pages contain OCR noise. Requirements that depend on those areas are still traced to the source pages, but they should receive a manual PDF check before being treated as final for conformance certification.
- Recommendations in the ECMA remarks sections are captured separately from strict conformance requirements where practical. They are useful for portability and behavior guidance, but they are not all mandatory.

## Suggested Next Steps

1. Map existing tests to these requirement IDs.
2. Create gap-analysis issues for uncovered requirement groups.
3. Add a conformance checklist that records parser/runtime/doc coverage per requirement.
