---
name: audit-Codex-config
description: Analyze .Codex configuration for gaps, improvements, and needed updates
user-invocable: true
argument-hint: ""
---

# Audit Codex Configuration Skill

Performs comprehensive analysis of the `.Codex/` directory and related configuration files, identifying gaps, suggesting improvements, and proposing updates based on project evolution.

## Usage

```bash
/audit-Codex-config
```

## What It Analyzes

### 1. Rules (`.Codex/rules/`)
- ✅ Are rules still relevant to current work?
- ✅ Do rules contradict each other?
- ✅ Are there new patterns that should be documented?
- ✅ Are rules loading at the right times (path patterns)?

### 2. Skills (`.Codex/skills/`)
- ✅ Are there repeated workflows that should be skills?
- ✅ Can existing skills be consolidated?
- ✅ Are skill descriptions accurate?
- ✅ Are there gaps in available commands?

### 3. Agents (`.Codex/agents/`)
- ✅ Are agents being used effectively?
- ✅ Should new specialized agents be created?
- ✅ Do agent tool restrictions make sense?
- ✅ Is agent memory growing effectively?

### 4. Agent Memory
- ✅ What have agents learned?
- ✅ Is memory becoming too large?
- ✅ Are patterns documented in rules?
- ✅ Should memory be compressed/archived?

### 5. Workflows
- ✅ Are generated workflows reusable?
- ✅ Should any be saved as named workflows?
- ✅ Are we using workflows appropriately?

### 6. Settings (`.Codex/settings.json`)
- ✅ Are permissions appropriate?
- ✅ Should hooks be added/modified?
- ✅ Are environment variables set correctly?

### 7. MCP Servers (`.mcp.json`)
- ✅ Should any be enabled?
- ✅ Are we manually doing what MCP could automate?

### 8. EditorConfig & Build Props
- ✅ Are code style rules being followed?
- ✅ Is Directory.Build.props up to date?
- ✅ Should new warnings be enforced?

## Output Format

```markdown
# 🔍 Codex Configuration Audit Report

Generated: [Date]

## 📊 Summary
- Rules: X active, Y need updates
- Skills: X available, Y suggestions
- Agents: X defined, Y utilized, Z suggestions
- Overall Health: ✅ Excellent | ⚠️ Good | ❌ Needs Attention

---

## ✅ What's Working Well

- [Strengths in current configuration]

---

## ⚠️ Identified Gaps

### Rules
1. **Missing Pattern Documentation**
   - Gap: We've fixed nullable warnings 10 times with same pattern
   - Recommendation: Add to `.Codex/rules/modernization.md`
   - Priority: Medium

### Skills
1. **Repeated Workflow**
   - Gap: Manually running build → test → coverage check frequently
   - Recommendation: Create `/pre-commit` skill
   - Priority: High

### Agents
1. **Underutilized Agent**
   - Gap: test-writer agent only used 2 times
   - Recommendation: Delegate more test writing tasks
   - Priority: Low

---

## 💡 Suggested Improvements

### High Priority
1. **Add nullable pattern to modernization.md**
   ```markdown
   Pattern: private readonly IConfig _config = null!;
   Reason: Constructor initialization, guaranteed non-null
   ```

2. **Create /pre-commit skill**
   Automates: build + test + coverage check

### Medium Priority
1. **Update code-reviewer agent**
   Add: Check for var keyword usage
   
2. **Enable GitHub MCP server**
   Use case: Automating issue creation

### Low Priority
1. **Consolidate similar skills**
   Combine: audit-build + verify-coverage into /pre-commit

---

## 📈 Agent Learning Summary

### code-reviewer
- Reviews completed: 5
- Patterns learned: 8
- Common issues: Nullable warnings, missing var keyword
- Effectiveness: ✅ High

### test-writer  
- Tests written: 12
- Coverage achieved: 87% average
- Patterns learned: 6
- Effectiveness: ✅ High

---

## 🎯 Recommended Actions

### Immediate (Do Now)
- [ ] Add nullable pattern to modernization.md
- [ ] Create /pre-commit skill

### Short-term (This Week)
- [ ] Update code-reviewer checklist
- [ ] Compress agent memory if > 100 entries

### Consider (Optional)
- [ ] Enable GitHub MCP server
- [ ] Create nullable-fixer specialized agent

---

## 📋 Configuration Health Checklist

- [✅] Rules load correctly and are relevant
- [✅] Skills are documented and user-invocable
- [⚠️] Agents could be used more frequently
- [✅] Agent memory is learning effectively
- [✅] Settings permissions are appropriate
- [❌] MCP servers might be beneficial but disabled
- [✅] EditorConfig rules are enforced
- [✅] Git conventions documented

---

## 📊 Metrics

- Total .Codex files: X
- Rules: X (Y with path restrictions)
- Skills: X available
- Agents: X defined, Y with memory
- Agent memory size: X KB
- Workflows generated: X
- Last audit: [Previous date] or Never

---

## 🔄 Next Audit Recommendation

Run `/audit-Codex-config` again:
- After 50 commits
- After 2 weeks
- When starting new feature area
- If configuration feels stale
```

## Analysis Process

I will:

1. **Read all configuration files**
   - Rules, skills, agents, memory
   - Settings, MCP, editorconfig

2. **Analyze git history**
   - Look for repeated patterns
   - Identify manual workflows
   - Check commit frequency

3. **Review agent memory**
   - What patterns have been learned?
   - Is learning transferring to rules?

4. **Check for gaps**
   - Missing documentation
   - Underutilized features
   - Automation opportunities

5. **Compare to best practices**
   - Are we following our own standards?
   - Are configurations optimal?

6. **Generate recommendations**
   - Prioritized by impact
   - Specific, actionable
   - With examples

## Use Cases

### Weekly Review
```
User: /audit-Codex-config
→ Review what's changed
→ Update configurations
→ Plan improvements
```

### After Major Milestone
```
User: "We just finished nullable fixes. /audit-Codex-config"
→ Document patterns learned
→ Update rules with discoveries
→ Archive completed work notes
```

### New Team Member Onboarding
```
User: /audit-Codex-config
→ Shows current state
→ Explains why things are configured this way
→ Identifies what's actively used
```

### Feeling Lost
```
User: "Configuration feels messy. /audit-Codex-config"
→ Identifies what's stale
→ Suggests cleanup
→ Proposes reorganization
```

## Output Saved To

Report saved to: `.Codex/audits/audit-{timestamp}.md`

## Related Skills

- `/verify-coverage` - Check test coverage
- `/audit-build` - Analyze build errors
- `/create-modernization-issue` - Create issues from findings

## Important Notes

- This skill is **read-only analysis** - I suggest changes but don't apply them
- You review recommendations and approve updates
- Agent memory is referenced but not modified
- Safe to run anytime - no side effects

## Implementation

When invoked, I will:

1. Read all `.Codex/` files
2. Analyze patterns and usage
3. Compare to best practices
4. Generate comprehensive report
5. Save report to `.Codex/audits/`
6. Present findings with recommendations
7. Ask: "Which improvements should I implement?"
