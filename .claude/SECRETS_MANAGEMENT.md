# Secrets Management

## Critical Security Rule

**NEVER commit secrets, tokens, or API keys to git.**

## Configuration Files

### `settings.json` - Committed to Git ✅
- Contains project-wide configuration
- Safe to commit (no secrets)
- Template for environment variables
- Tracked by version control

### `settings.local.json` - Gitignored 🔒
- Contains user-specific secrets
- **Automatically gitignored**
- Overrides values from `settings.json`
- Never committed

## How It Works

Claude Code loads configuration in this order (later overrides earlier):

1. `.claude/settings.json` (project defaults, committed)
2. `.claude/settings.local.json` (user secrets, gitignored)
3. System environment variables (highest priority)

## Setup for GitHub Token

### Option 1: Local Settings File (Recommended for Development)

Create `.claude/settings.local.json`:
```json
{
  "environment": {
    "GITHUB_TOKEN": "your_token_here"
  }
}
```

**This file is automatically gitignored and safe.**

### Option 2: System Environment Variable (Recommended for CI/CD)

**Windows (PowerShell)**:
```powershell
[System.Environment]::SetEnvironmentVariable('GITHUB_TOKEN', 'your_token_here', 'User')
```

**Windows (Command Prompt)**:
```cmd
setx GITHUB_TOKEN "your_token_here"
```

**Linux/macOS (Bash)**:
```bash
# Add to ~/.bashrc or ~/.zshrc
export GITHUB_TOKEN="your_token_here"
```

**GitHub Actions**:
```yaml
env:
  GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

### Option 3: Get from GitHub CLI (Automatic)

Claude Code can retrieve the token from `gh` CLI automatically:
```bash
gh auth token
```

This is the **safest** option as the token is managed by GitHub CLI's credential store.

## Verification

### Check if Token is Set
```bash
# Check environment variable
echo $GITHUB_TOKEN

# Or use gh CLI
gh auth status
```

### Test MCP Server Connection
```bash
# Will show error if token not found
# (Claude Code will attempt this automatically)
```

## Gitignore Protection

The following patterns protect secrets in `.gitignore`:

```gitignore
# Claude Code - Secrets Protection
.claude/settings.local.json      # User-specific secrets
.claude/**/*.secret              # Any .secret files
.claude/**/*.key                 # Any .key files
.env                             # Environment files
.env.*                           # .env variants
!.env.example                    # Allow .env.example template
```

## What's Safe to Commit?

✅ **Safe to Commit**:
- `.claude/settings.json` (no secrets)
- `.claude/rules/*.md`
- `.claude/skills/*/SKILL.md`
- `.claude/agents/*.md`
- `.mcp.json` (if no hardcoded tokens)
- Configuration templates

❌ **NEVER Commit**:
- `.claude/settings.local.json`
- Files with actual tokens/keys
- `.env` files with secrets
- Any file matching `*.secret` or `*.key`

## CI/CD Setup

### GitHub Actions

Use GitHub Secrets (Settings → Secrets and variables → Actions):

```yaml
name: CI

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    env:
      GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}  # Automatic
      # Or use a custom secret:
      # GITHUB_TOKEN: ${{ secrets.MY_GITHUB_PAT }}
```

**Note**: `secrets.GITHUB_TOKEN` is automatically provided by GitHub Actions.

### Other CI Systems

Set `GITHUB_TOKEN` as a **secure environment variable** in your CI system:
- GitLab CI: Settings → CI/CD → Variables
- Azure DevOps: Pipelines → Library → Variable groups
- Jenkins: Credentials → Secret text

## Token Generation

Generate a GitHub Personal Access Token at:
https://github.com/settings/tokens

**Required scopes for GitHub MCP server**:
- `repo` (full control of private repositories)
- `read:org` (read organization membership)
- `admin:public_key` (optional, for deploy keys)

**Token expiration**: 
- Development: 90 days (renew as needed)
- CI/CD: No expiration (use GitHub App tokens instead)

## Rotating Tokens

If a token is compromised:

1. **Revoke immediately** at https://github.com/settings/tokens
2. **Generate new token** with same scopes
3. **Update in secure location**:
   - `.claude/settings.local.json`, OR
   - System environment variable, OR
   - CI/CD secrets
4. **Verify** new token works: `gh auth status`

## Security Best Practices

1. ✅ **Use separate tokens** for dev vs CI/CD
2. ✅ **Rotate tokens** every 90 days
3. ✅ **Limit token scopes** to minimum required
4. ✅ **Never log tokens** in build output
5. ✅ **Use GitHub Apps** for automation (better than PATs)
6. ✅ **Review token usage** periodically
7. ✅ **Revoke unused tokens** immediately

## Troubleshooting

### "GITHUB_TOKEN not found"

Check in order:
1. `.claude/settings.local.json` exists and has token
2. System environment variable is set: `echo $GITHUB_TOKEN`
3. GitHub CLI is authenticated: `gh auth status`
4. Restart terminal/IDE after setting environment variables

### "Authentication failed"

- Token may be expired (check https://github.com/settings/tokens)
- Token may lack required scopes (`repo` needed)
- Token may have been revoked

### "MCP server not starting"

- Check `.mcp.json` has `"disabled": false` for github server
- Verify `npx` command works: `npx -y @modelcontextprotocol/server-github --version`
- Check Claude Code logs for errors

## References

- [GitHub Personal Access Tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)
- [GitHub Apps vs PATs](https://docs.github.com/en/developers/apps/getting-started-with-apps/about-apps#personal-access-tokens-vs-github-apps)
- [Claude Code MCP Documentation](https://github.com/modelcontextprotocol/servers/tree/main/src/github)

## Summary

🔒 **Security Rule**: Secrets go in `.claude/settings.local.json` (gitignored) or environment variables, NEVER in `.claude/settings.json` (committed).

✅ **Current Setup**: 
- `.gitignore` protects `settings.local.json`
- Token stored in gitignored file
- MCP server configured to use `${GITHUB_TOKEN}`
- Safe to commit all other `.claude/` files
