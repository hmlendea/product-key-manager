# Contributing to Product Key Manager

This document covers the guidelines and processes for contributing to this project, including how to report issues, suggest enhancements, submit code changes, and follow project standards.

## 📑 Table of Contents

- [How to Contribute](#how-to-contribute)
- [Code of Conduct](#code-of-conduct)
- [Security](#security)
- [License](#license)
- [Getting Assistance](#getting-assistance)

## 🤝 How to Contribute

### Reporting Issues

- Search existing issues first.
- Use the issue templates if available.
- Provide clear reproduction steps.
- Include environment details.

### Suggesting Enhancements

- Check the roadmap and existing discussions.
- Explain the use case and expected behaviour.
- Consider implementation complexity.

### Code Contributions

#### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

#### Development Setup

```bash
# Clone the repository
git clone https://github.com/hmlendea/product-key-manager.git
cd product-key-manager

# Install dependencies
dotnet restore ProductKeyManager.slnx
```

#### Making Changes

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes.
4. Run tests: `dotnet test ProductKeyManager.slnx`
5. Run linters: `dotnet build ProductKeyManager.slnx`
6. Commit with clear and descriptive messages.
7. Push to your fork.
8. Open a Pull Request.

#### Pull Request Guidelines

- Target the `master` branch.
- Keep PRs focused and atomic.
- Revise documentation if applicable.
- Include tests for any new functionality.
- Ensure the CI checks pass.

### Code Style

Follow the project's coding standards:
- Target .NET 10.0 with nullable reference types enabled
- Use explicit types, no `var`
- Use expression-bodied members for single-statement methods
- Follow the existing naming conventions (PascalCase for public members, camelCase for private fields)
- Run `dotnet build ProductKeyManager.slnx` before committing to verify compilation

### Testing

```bash
# Run all tests
dotnet test ProductKeyManager.slnx

# Run specific test suite
dotnet test ProductKeyManager.UnitTests/ProductKeyManager.UnitTests.csproj
dotnet test ProductKeyManager.IntegrationTests/ProductKeyManager.IntegrationTests.csproj
```

### Documentation

- Revise relevant docs for changes.
- Follow the documentation style guide.
- Preview changes locally if possible.

## 📋 Code of Conduct

This project follows the [Code of Conduct](CODE_OF_CONDUCT.md).

## 🔒 Security

Report security vulnerabilities per the [Security Policy](SECURITY.md).

## 📄 License

By contributing, you agree that your contributions will be licensed under the GNU General Public License v3.0.

## ❓ Getting Assistance

- Open an issue on GitHub for questions or problems
- Check existing discussions and documentation first.