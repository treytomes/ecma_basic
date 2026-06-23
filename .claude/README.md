# Claude Code Configuration

This directory contains configuration for Claude Code (claude.ai/code) to assist with development of this project.

## 🔒 Security First

**CRITICAL**: See [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md) for how to handle tokens and API keys safely.

- ✅ `settings.json` - Safe to commit (no secrets)
- 🔒 `settings.local.json` - **Gitignored** (contains secrets, NEVER commit)

## Directory Structure

```
.claude/
├── README.md                    # This file
├── SECRETS_MANAGEMENT.md        # 🔒 How to handle tokens safely
├── settings.json                # ✅ Project config (committed)
├── settings.local.json          # 🔒 User secrets (gitignored)
├── rules/                       # Development standards
│   ├── modernization.md
│   ├── testing.md
│   ├── spec-first.md
│   ├── github-workflow.md
│   └── git-conventions.md
├── skills/                      # User-invocable commands
│   ├── verify-coverage/
│   ├── audit-build/
│   ├── audit-claude-config/
│   ├── create-modernization-issue/
│   └── commit-and-push/
├── agents/                      # Specialized subagents
│   ├── code-reviewer.md
│   └── test-writer.md
├── agent-memory/                # Agent learning storage
│   ├── code-reviewer/
│   └── test-writer/
├── workflows/                   # Multi-agent orchestration
├── output-styles/               # Response formatting
└── audits/                      # Configuration audit reports
```

## Quick Start

### Using Skills (Slash Commands)

```bash
/verify-coverage                      # Check test coverage
/audit-build                          # Analyze build errors
/audit-claude-config                  # Review configuration
/create-modernization-issue "Title"   # Create GitHub issue
/commit-and-push                      # Smart commit workflow
```

### Delegating to Agents

```bash
"Have the code-reviewer agent review this file"
"Ask the test-writer agent to create tests for X class"
```

### Running Workflows

```bash
"Use a workflow to fix nullable warnings across all Core files"
```

## Configuration Files

### settings.json (Committed)
- Permissions (full repo access)
- Build verification hooks
- Default environment variables
- **No secrets**

### settings.local.json (Gitignored) 🔒
- User-specific secrets (GITHUB_TOKEN, etc.)
- Local environment overrides
- **Automatically created on first use**
- **Never committed to git**

## Security

All sensitive files are protected by `.gitignore`:
- `.claude/settings.local.json`
- `.claude/**/*.secret`
- `.claude/**/*.key`
- `.env` and `.env.*`

See [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md) for complete security guide.

## Documentation

- [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md) - Token and secrets handling
- [rules/README.md](rules/README.md) - Development standards
- [skills/README.md](skills/README.md) - Available commands
- [agents/README.md](agents/README.md) - Specialized agents
- [workflows/README.md](workflows/README.md) - Multi-agent orchestration

## Maintenance

Run periodic configuration audits:
```bash
/audit-claude-config
```

This checks:
- Rule relevance
- Skill effectiveness
- Agent utilization
- Memory growth
- Configuration health

## Getting Help

- Check skill documentation: `/create-modernization-issue --help`
- Read CLAUDE.md in project root for development guide
- Consult specific rule files in `.claude/rules/`
- Ask Claude directly: "How do I...?"
