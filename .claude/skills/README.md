# ECMABasic Skills

Skills are reusable workflows that can be invoked with slash commands. Each skill automates common development tasks.

## Available Skills

### 🚀 `/commit-and-push`
Build, test, intelligently group changes, commit with Conventional Commits format, and push.

**Usage**: `/commit-and-push`

**What it does**:
- Verifies build succeeds (zero warnings)
- Runs all tests (must pass)
- Checks coverage ≥ 80%
- Groups changed files logically
- Generates Conventional Commit messages
- Presents each commit for approval
- Pushes to remote after confirmation

**Safety features**:
- Won't commit if build fails
- Won't commit if tests fail
- Won't commit if coverage < 80%
- Approval required for each commit
- Confirmation required before push

**Example grouping**:
- Source code changes → One commit
- Test updates → Separate commit  
- Documentation → Separate commit
- Configuration → Separate commit

---

### 📊 `/verify-coverage`
Run tests and verify code coverage meets the 80% minimum requirement.

**Usage**: `/verify-coverage`

**What it does**:
- Runs all tests with code coverage
- Parses coverage percentage
- Fails if < 80%, succeeds if ≥ 80%

---

### 📝 `/create-modernization-issue`
Create a properly formatted GitHub issue for modernization tasks.

**Usage**: `/create-modernization-issue "Issue Title"`

**Example**: `/create-modernization-issue "Fix nullable warnings in CharacterReader"`

**What it does**:
- Loads appropriate template (nullable, file-scoped namespace, etc.)
- Generates formatted issue markdown
- Provides ready-to-paste content for GitHub

**Templates included**:
- `nullable.md` - For CS86xx nullable reference type errors
- `file-scoped-namespace.md` - For namespace conversion

---

### 🔍 `/audit-build`
Analyze build errors and categorize them by type for planning fixes.

**Usage**: `/audit-build`

**What it does**:
- Builds the solution
- Captures all errors and warnings
- Categorizes by type (nullable, style, other)
- Groups by file
- Provides prioritized recommendations

**Output includes**:
- Error counts by category
- Most affected files
- Detailed breakdown by error code
- Priority recommendations
- Effort estimates

---

### 🔧 `/audit-claude-config`
Analyze `.claude/` configuration for gaps and improvement opportunities.

**Usage**: `/audit-claude-config`

**What it does**:
- Reviews all rules, skills, agents, memory
- Identifies repeated patterns that should be documented
- Suggests new skills for repeated workflows
- Analyzes agent effectiveness and learning
- Proposes configuration improvements
- Saves comprehensive report

**Output includes**:
- Configuration health summary
- What's working well
- Identified gaps
- Prioritized recommendations
- Agent learning summary
- Actionable next steps

**When to run**:
- Weekly maintenance
- After major milestones
- When configuration feels stale
- Before/after new team member onboarding

---

## Creating New Skills

1. Create a folder: `.claude/skills/skill-name/`
2. Add `SKILL.md` with frontmatter:
   ```markdown
   ---
   name: skill-name
   description: What it does
   user-invocable: true
   argument-hint: <optional-args>
   ---
   
   # Content here
   ```
3. Add supporting files (templates, scripts, etc.)
4. Invoke with `/skill-name`

## Skill Ideas for ECMABasic

Additional skills that could be useful:

- `/prepare-release <version>` - Pre-release checklist and automation
- `/update-dependencies` - Check and update NuGet packages
- `/run-sample <program.bas>` - Load and run a sample BASIC program
- `/benchmark` - Run performance benchmarks
- `/lint` - Run all code quality checks
- `/create-test` - Generate test template for a class
- `/wiki-sync` - Sync documentation to GitHub wiki

## Variables in Skills

- `$ARGUMENTS` - All arguments as string
- `$0`, `$1`, `$2` - Positional arguments
- `` !`command` `` - Execute shell command inline

## See Also

- [Rules Documentation](../rules/) - Automated guidance and constraints
- [CLAUDE.md](../../CLAUDE.md) - Development guidelines
- [Skills Documentation](https://docs.claude.ai/skills) - Official docs
