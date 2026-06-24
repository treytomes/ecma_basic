# TODO

## In Progress

* Issue #32: ✅ **COMPLETE** - Spectre.Console research complete (deferred in favor of ANSI codes)
  - Research documented in `.claude/research/spectre-console-evaluation.md`
  - Follow-up Issue #44 created for ANSI color implementation

## Next

* ✅ **COMPLETE** - Gap analysis comparing ECMABasic55 with ECMA-55 requirements
  - See: `.claude/audits/ecma55-gap-analysis-2026-06-24.md`
  - Missing features tracked as Issues #34-40 (v0.4 milestone)
* `docs\ECMA-6, 7-Bit Coded Character Set.pdf` should be reviewed for any relevant requirements.  What is the purpose of the document?  Should it's implementation wait for a later version?  I'm considering a Godot-based presentation layer, but not until this project is more complete.

## Future

* A Godot presentation layer could include a virtual disk system that can read and write `.DSK` image files.
* Within Godot, implement retro shader effects for scan lines and fading out the corners of the screen to mimic an old TV display.
* Implement an old-school terminal using the Extended ASCII character set.  Maybe 80x25?  Hold the screen to the aspect ratio you might expect from a TV or monitor in the late 80s.
