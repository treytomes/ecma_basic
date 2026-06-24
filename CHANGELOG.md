# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- GitHub Actions CI/CD pipeline with automated testing and coverage reporting
- Code coverage requirement (minimum 80%)
- Release automation via GitHub Actions

### Changed
- Updated project structure to follow Clean Architecture principles
- Modernized development guidelines with spec-driven development approach
- Enhanced documentation with GitHub integration (issues for specs, wiki for docs)

## [0.2.0] - 2022-XX-XX

### Added
- ECMA-55 Minimal BASIC implementation
- Interactive REPL environment
- Support for BASIC statements: LET, PRINT, GOTO, GOSUB, IF-THEN, FOR-NEXT, READ-DATA, INPUT
- Immediate-mode commands: RUN, LIST, NEW, LOAD, SAVE, CONTINUE
- Sample programs demonstrating language features
- xUnit test suite with ECMA-55 compliance tests

### Technical
- .NET 6.0 target framework
- Parser-based interpreter architecture
- Extensible statement and expression system
- Configuration system for BASIC dialect settings

## [0.1.0] - Initial Release

### Added
- Basic interpreter framework
- Core parsing and tokenization
- Console environment implementation
