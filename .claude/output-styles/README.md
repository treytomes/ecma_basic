# Output Styles

Output styles customize how Claude Code formats responses.

## What Are Output Styles?

Output styles control:
- Message structure and formatting
- Use of headers, lists, code blocks
- Verbosity level
- Technical depth

## Default Style

The default output style is defined in Claude Code's core settings. It's suitable for most development work.

## When to Customize

Consider custom output styles for:
- **Terse mode**: Minimal output for experienced developers
- **Verbose mode**: Detailed explanations for learning
- **Report mode**: Structured, formal documentation
- **Debug mode**: Include reasoning and decision process

## Creating a Custom Style

1. Create a markdown file: `style-name.md`
2. Define the style with frontmatter and instructions
3. Activate via settings: `"outputStyle": "style-name"`

## Example: Terse Style

```markdown
---
name: terse
description: Minimal output for experienced developers
---

# Terse Output Style

Format responses as:
- One-line summary of action taken
- Code diffs only (no explanation)
- Errors with file:line format
- No emojis, no headers, no fluff

Example:
Fixed nullable warning in CharacterReader.cs:44
```

## Example: Learning Style

```markdown
---
name: learning
description: Detailed explanations for educational context
---

# Learning Output Style

Format responses as:
- Explain WHY before WHAT
- Include code examples with annotations
- Reference ECMA-55 standard sections
- Suggest related concepts to explore
- Use analogies and comparisons
```

## Current Status

**Using default style** - suitable for professional development.

## Recommendation

For this project, the **default style is appropriate** because:
- You're an experienced developer
- Project is well-structured with clear guidelines
- Focus is on modernization, not learning
- Standard technical communication works well

### If You Want Customization

I can create:
- **`spec-focused.md`** - Always reference GitHub issues/specs
- **`architecture-aware.md`** - Include Clean Architecture impact
- **`coverage-conscious.md`** - Mention test coverage in every response

Let me know if you'd like any of these!

## Activation

To activate a custom style, edit `.claude/settings.json`:

```json
{
  "outputStyle": "style-name"
}
```

## Related

- `.claude/settings.json` - Configure which style to use
- Global output styles: `~/.claude/output-styles/`
