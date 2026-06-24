# ECMA-55 Gap Analysis

**Date**: 2026-06-24  
**Specification Source**: `docs/specifications/ecma-55/requirements.md` (verified against Wikipedia)  
**Standard**: ECMA-55 Minimal BASIC (1978) - NOT ECMA-116

## Verification Summary

✅ **Specification Completeness Confirmed**:
- Wikipedia cross-reference validates all 11 intrinsic functions
- Confirms batch (non-interactive) execution requirement (ECMA55-GEN-003)
- Validates all statement types and language constraints

## Missing Features (Priority Order)

### 🔴 HIGH PRIORITY - Required by ECMA-55

#### 1. Missing Intrinsic Functions (ECMA55-FUN-001)

**Required but NOT implemented**:
- `ATN(x)` - Arctangent
- `EXP(x)` - Exponential (e^x)
- `LOG(x)` - Natural logarithm  
- `SQR(x)` - Square root

**Currently implemented**: ABS, COS, INT, RND, SGN, SIN, TAN (7/11)

**Requirements**:
- `ECMA55-FUN-001`: All 11 functions must be provided
- `ECMA55-FUN-005`: LOG(x) with x≤0 raises fatal exception
- `ECMA55-FUN-006`: SQR(x) with x<0 raises fatal exception
- `ECMA55-FUN-007`: EXP/TAN overflow raises nonfatal exception

**Location**: `src/ECMABasic.Application/FunctionFactory.cs` (TODO comment exists)

---

#### 2. User-Defined Functions - DEF FN (ECMA55-DEF-001 to ECMA55-DEF-008)

**Status**: ❌ NOT implemented

**Requirements**:
- `DEF FNx = expression` - zero-parameter function
- `DEF FNx(param) = expression` - one-parameter function
- Parser validation: definition before use, no recursion, define once
- Runtime: local parameter scope, no effect when executed directly

**Estimated effort**: 8-12 hours (parser + statement + execution)

---

#### 3. Array Support - DIM and OPTION BASE (ECMA55-ARR-001 to ECMA55-ARR-009)

**Status**: ❌ NOT implemented

**Requirements**:
- `DIM A(n)` - 1D array with bounds 0 to n (or 1 to n with OPTION BASE 1)
- `DIM A(m,n)` - 2D array
- `OPTION BASE 0` or `OPTION BASE 1` - set array lower bound
- Default allocation: 0-10 (or 1-10 with BASE 1) when DIM absent
- Parser validation: DIM before use, OPTION BASE before DIM, define once

**Estimated effort**: 12-16 hours (parser + runtime array storage + bounds checking)

---

#### 4. RANDOMIZE Statement (ECMA55-RND-001, ECMA55-RND-002)

**Status**: ❌ NOT implemented

**Requirements**:
- `RANDOMIZE` seeds RND with unpredictable value
- Without RANDOMIZE: same program = same RND sequence (ECMA55-FUN-008)
- Current RND implementation may not guarantee repeatability

**Estimated effort**: 2-3 hours (statement + RandomFactory seed control)

---

#### 5. Batch Mode Execution (ECMA55-GEN-003)

**Status**: ⚠️ PARTIAL - only interactive REPL exists

**Requirement**: "Interactive use shall not be assumed as the only supported execution model"

**Current**: ECMABasic55 REPL requires interactive terminal

**Needed**: 
- Non-interactive file execution mode
- Batch INPUT handling (ECMA55-DOC-014)
- Redirect stdin/stdout support

**Estimated effort**: 4-6 hours (command-line arg parsing + non-interactive environment)

---

### 🟡 MEDIUM PRIORITY - Implementation Details

#### 6. Conformance Documentation (ECMA55-DOC-001 to ECMA55-DOC-014)

**Status**: ❌ NOT documented

**Requirements**: Reference manual must define:
- Numeric precision/range (DOC-001, DOC-010)
- Initial variable values (DOC-004)
- Machine infinitesimal/infinity (DOC-007, DOC-008)
- Print formatting (DOC-003, DOC-009, DOC-011, DOC-013)
- End-of-line handling (DOC-002)
- Input prompt format (DOC-005)
- Max string length (DOC-006) - currently 18 chars per config
- Batch input handling (DOC-014)
- RND sequence definition (DOC-012)

**Estimated effort**: 8-10 hours (create reference manual documentation)

---

#### 7. RND Semantics Verification (ECMA55-FUN-004)

**Status**: ⚠️ NEEDS VERIFICATION

**Current implementation**: 
```csharp
Define("RND", [ExpressionType.Number], args => RandomFactory.Instance.Next(Convert.ToInt32(args[0])));
```

**ECMA-55 requirement**: "RND shall return... uniformly distributed over `0 <= RND < 1`"

**Problem**: ECMA-55 RND is **parameterless** and returns 0-1 range. Current implementation takes parameter.

**Note**: Wikipedia confirms "RND" with no arguments returns [0,1). Implementation appears to follow a non-standard dialect.

**Estimated effort**: 2-3 hours (verify spec, fix if needed, add tests)

---

### 🟢 LOW PRIORITY - Optional Extensions

#### 8. String Functions (NOT in ECMA-55)

**Status**: ❌ NOT in ECMA-55 Minimal BASIC

Wikipedia confirms: "No built-in or user-defined string functions" in ECMA-55.

**Current extensions** (Program.cs InjectIntrinsics):
- `ASC`, `MID$`, `POS` - These are **NOT ECMA-55 compliant**

**Decision needed**: 
- Remove non-standard functions for ECMA-55 conformance?
- OR label as "ECMA-55 + extensions" implementation?

---

#### 9. SLEEP Statement (NOT in ECMA-55)

**Found**: `src/ECMABasic55/Parsers/SleepStatementParser.cs`

**Status**: Extension - NOT in ECMA-55

---

## Implementation Coverage Summary

| Category | Requirement IDs | Status | Coverage |
|----------|----------------|--------|----------|
| **Intrinsic Functions** | ECMA55-FUN-001 to FUN-008 | ⚠️ Partial | 7/11 functions (64%) |
| **User-Defined Functions** | ECMA55-DEF-001 to DEF-008 | ❌ Missing | 0/8 requirements (0%) |
| **Arrays** | ECMA55-ARR-001 to ARR-009 | ❌ Missing | 0/9 requirements (0%) |
| **RANDOMIZE** | ECMA55-RND-001 to RND-002 | ❌ Missing | 0/2 requirements (0%) |
| **Batch Execution** | ECMA55-GEN-003 | ⚠️ Partial | Interactive only |
| **Documentation** | ECMA55-DOC-001 to DOC-014 | ❌ Missing | 0/14 requirements (0%) |
| **Statements** | All control/I/O statements | ✅ Complete | ~95% (missing only DEF, DIM, OPTION, RANDOMIZE) |
| **Expressions** | ECMA55-EXP-001 to EXP-012 | ✅ Complete | 100% |
| **Variables** | ECMA55-VAR-001 to VAR-012 | ⚠️ Partial | No arrays |

**Overall ECMA-55 Conformance**: ~60% (core statements good, missing arrays/DEF/docs)

---

## Recommended Issue Creation Order

### Phase 1: Core Language Features
1. **Issue: Implement missing intrinsic functions (ATN, EXP, LOG, SQR)**
   - Requirements: ECMA55-FUN-001, FUN-005, FUN-006, FUN-007
   - Effort: 3-4 hours
   - Priority: HIGH
   - Blocks: Nothing, standalone feature

2. **Issue: Fix RND function to match ECMA-55 specification**
   - Requirements: ECMA55-FUN-004, FUN-008
   - Effort: 2-3 hours
   - Priority: HIGH
   - Blocks: RANDOMIZE implementation

3. **Issue: Implement RANDOMIZE statement**
   - Requirements: ECMA55-RND-001, RND-002
   - Effort: 2-3 hours
   - Priority: HIGH
   - Depends: RND fix

### Phase 2: Advanced Features
4. **Issue: Implement DEF FN user-defined functions**
   - Requirements: ECMA55-DEF-001 to DEF-008
   - Effort: 8-12 hours
   - Priority: MEDIUM
   - Complex: Parser + scoping + validation

5. **Issue: Implement DIM and OPTION BASE for arrays**
   - Requirements: ECMA55-ARR-001 to ARR-009
   - Effort: 12-16 hours
   - Priority: MEDIUM
   - Complex: Multi-dimensional storage + bounds checking

### Phase 3: Non-Interactive Support
6. **Issue: Add batch (non-interactive) execution mode**
   - Requirements: ECMA55-GEN-003, DOC-014
   - Effort: 4-6 hours
   - Priority: MEDIUM
   - Enables: Automated testing, scripting

### Phase 4: Documentation
7. **Issue: Create ECMA-55 conformance reference manual**
   - Requirements: ECMA55-DOC-001 to DOC-014, CNF-007
   - Effort: 8-10 hours
   - Priority: LOW
   - Required for: Conformance certification

---

## Notes

- Current implementation includes **non-ECMA-55 extensions**: ASC, MID$, POS, SLEEP
- Decision needed: Strict ECMA-55 mode vs. "ECMA-55 + extensions"
- FunctionFactory refactoring (Issue #30) should be done BEFORE adding new functions
- Test coverage target: 80% maintained for all new features

---

## Next Steps

1. ✅ Create GitHub issues for Phase 1 (intrinsic functions)
2. ✅ Create GitHub issues for Phase 2 (DEF FN, arrays)
3. ✅ Create GitHub issue for batch mode support
4. Review non-standard extensions policy with maintainer
5. Begin implementation starting with missing intrinsic functions
