---
name: github-workflow
description: GitHub-specific workflows for issues, wiki, and documentation
---

# GitHub Workflow

## Issue-Driven Development

### Specifications as Issues
All features and bugs are tracked as GitHub issues:
- **Feature Requests**: Create issues with `enhancement` label
- **Bug Reports**: Create issues with `bug` label
- **Acceptance Criteria**: Define testable requirements in issue description
- **Test References**: Link test code to originating issue number

### Issue Template
When creating feature issues:
```markdown
## Description
[Clear description of the feature]

## Acceptance Criteria
- [ ] Criterion 1 (testable)
- [ ] Criterion 2 (testable)
- [ ] Test coverage ≥ 80%
- [ ] Documentation updated

## Technical Notes
[Implementation considerations]
```

### Linking Issues
- Commit messages: `Fixes #123` or `Closes #456` to auto-close issues
- Test documentation: Reference issue in XML comments
- Pull requests: Link related issues in PR description

## Wiki Documentation

The GitHub wiki is the source of truth for project documentation:

### Wiki Structure
- **Home**: Project overview and quick start
- **Architecture**: Design decisions and system structure
- **ECMA Standards**: Notes on ECMA-55 and ECMA-116 implementation
- **Development Guide**: Setup, building, testing
- **Language Reference**: BASIC syntax and commands
- **Roadmap**: Future plans and version history

### When to Update Wiki
- Architecture decisions (record ADRs)
- New features with user documentation
- Breaking changes or migration guides
- Performance optimizations and benchmarks
- Common troubleshooting scenarios

### Updating the Wiki
```bash
# Clone the wiki repository
git clone https://github.com/treytomes/ecma_basic.wiki.git

# Edit markdown files
# Commit and push changes

# Wiki changes appear immediately on GitHub
```

## Pull Request Workflow

### Creating PRs
- Branch from `main`: `git checkout -b feature/issue-123-description`
- Reference issue: "Closes #123" in PR description
- Ensure CI passes (build, tests, coverage)
- Request review from maintainers

### PR Checklist
- [ ] All tests pass
- [ ] Code coverage ≥ 80%
- [ ] No compiler warnings
- [ ] XML documentation complete
- [ ] CHANGELOG.md updated (for releases)
- [ ] Wiki updated (if needed)

## Continuous Integration

GitHub Actions automatically:
- Build solution on push/PR
- Run all tests
- Measure code coverage
- Report coverage to PR
- Block merge if coverage < 80%

## Release Process

### Versioning
Follow Semantic Versioning (SemVer):
- **Major** (1.0.0): Breaking changes
- **Minor** (0.1.0): New features, backward compatible
- **Patch** (0.0.1): Bug fixes

### Release Steps
1. Update version in `.csproj` files
2. Update `CHANGELOG.md` with release notes
3. Create git tag: `git tag v0.3.0`
4. Push tag: `git push origin v0.3.0`
5. GitHub Actions builds and creates release
6. Binaries attached to GitHub release

## Labels

Standard labels for issues and PRs:
- `bug`: Something isn't working
- `enhancement`: New feature or improvement
- `documentation`: Documentation updates
- `good first issue`: Good for newcomers
- `help wanted`: Extra attention needed
- `question`: Further information requested
- `wontfix`: Won't be worked on

## Project Board

Use GitHub Projects for sprint planning:
- **Backlog**: Prioritized issues to work on
- **In Progress**: Currently being developed
- **In Review**: Pull request open
- **Done**: Merged and closed
