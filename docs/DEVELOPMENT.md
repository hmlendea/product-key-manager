# Product Key Manager Development Guide

This document describes how to set up, build, test, and contribute to the Product Key Manager project.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git
- A code editor (VS Code, Visual Studio, JetBrains Rider, etc.)

## Repository Structure

```
product-key-manager/
├── ProductKeyManager/                 # Main ASP.NET Core application
│   ├── Api/                           # HTTP API layer
│   │   ├── Controllers/               # API controllers
│   │   └── Models/                    # Request/response models
│   ├── Configuration/                 # Configuration classes
│   ├── DataAccess/                    # Data persistence layer
│   │   └── DataObjects/               # XML data objects
│   ├── Logging/                       # Structured logging vocabulary
│   ├── Service/                       # Business logic layer
│   │   ├── Mapping/                   # Model transformations
│   │   └── Models/                    # Domain models
│   ├── appsettings.json               # Configuration file
│   ├── Program.cs                     # Application entry point
│   ├── Startup.cs                     # Service configuration
│   └── ServiceCollectionExtensions.cs # DI registration
├── ProductKeyManager.UnitTests/       # Unit tests
├── ProductKeyManager.IntegrationTests/ # Integration tests
├── docs/                              # Documentation
├── ARCHITECTURE.md                    # Architecture documentation
├── PRIVACY.md                         # Privacy policy
├── SECURITY.md                        # Security policy
├── ProductKeyManager.slnx             # Solution file
└── release.sh                         # Release automation
```

## Setup

### Clone the Repository

```bash
git clone https://github.com/hmlendea/product-key-manager.git
cd product-key-manager
```

### Restore Dependencies

```bash
dotnet restore ProductKeyManager.slnx
```

### Configure Settings

Copy the example configuration and update the shared secret:

```bash
cp ProductKeyManager/appsettings.json ProductKeyManager/appsettings.Development.json
```

Edit `ProductKeyManager/appsettings.Development.json` and set a secure value for `securitySettings.sharedSecretKey`.

## Building

### Build the Solution

```bash
dotnet build ProductKeyManager.slnx
```

### Build Specific Project

```bash
dotnet build ProductKeyManager/ProductKeyManager.csproj
```

## Running

### Run the Application

```bash
dotnet run --project ProductKeyManager
```

The service will start on `https://localhost:5001` and `http://localhost:5000` by default.

### Run with Watch Mode (Hot Reload)

```bash
dotnet watch run --project ProductKeyManager
```

## Testing

### Run All Tests

```bash
dotnet test ProductKeyManager.slnx
```

### Run Unit Tests Only

```bash
dotnet test ProductKeyManager.UnitTests/ProductKeyManager.UnitTests.csproj
```

### Run Integration Tests Only

```bash
dotnet test ProductKeyManager.IntegrationTests/ProductKeyManager.IntegrationTests.csproj
```

### Run Tests with Coverage

```bash
dotnet test ProductKeyManager.slnx --collect:"XPlat Code Coverage"
```

### Run Specific Test Class

```bash
dotnet test ProductKeyManager.UnitTests/ProductKeyManager.UnitTests.csproj --filter "FullyQualifiedName~ProductKeyServiceTests"
```

## Test Structure

### Unit Tests

Located in `ProductKeyManager.UnitTests/`:
- `Service/ProductKeyServiceTests.cs` - Tests for product key service logic
- `Api/Models/` - Tests for API model serialization and validation

### Integration Tests

Located in `ProductKeyManager.IntegrationTests/`:
- `ProductKeysApiIntegrationTests.cs` - HTTP API CRUD and filtering tests
- `ProductKeysApiSecurityIntegrationTests.cs` - Security and authentication tests
- `ProductKeysApiPersistenceIntegrationTests.cs` - XML persistence tests
- `ProductKeyXmlPersistenceIntegrationTests.cs` - Repository behavior tests
- `Infrastructure/` - Test infrastructure (factory, base classes, request factory)

### Test Infrastructure

- `ProductKeyManagerApiFactory` - Creates test host with isolated XML stores
- `ProductKeyManagerApiTestBase` - Base class with common test setup
- `ProductKeyRequestFactory` - Creates properly signed HTTP requests
- `ProductKeyApiResponseAssertions` - Assertion helpers for API responses

## Development Workflow

### Making Changes

1. Create a feature branch from `master`
2. Make your changes
3. Run tests to ensure nothing is broken
4. Update documentation if needed
5. Submit a pull request

### Code Style

The project follows standard C# conventions:
- Use `var` for local variables when type is obvious
- Use expression-bodied members for simple methods/properties
- Follow Microsoft C# naming conventions
- Use nullable reference types (enabled by default in .NET 10)

### Adding New Features

1. **API Changes:** Add/modify models in `ProductKeyManager/Api/Models/`
2. **Business Logic:** Implement in `ProductKeyManager/Service/ProductKeyService.cs`
3. **Persistence:** Modify `ProductKeyManager/DataAccess/` if schema changes
4. **Tests:** Add unit tests in `ProductKeyManager.UnitTests/` and integration tests in `ProductKeyManager.IntegrationTests/`

### Debugging

#### Debug in VS Code

1. Open the folder in VS Code
2. Install the C# Dev Kit extension
3. Press F5 to start debugging

#### Debug in Visual Studio

1. Open `ProductKeyManager.slnx`
2. Set `ProductKeyManager` as startup project
3. Press F5 to start debugging

## Common Tasks

### Adding a New Product Key Status

1. Add the status to `ProductKeyManager/Service/Models/ProductKeyStatus.cs`
2. Update tests to cover the new status
3. Update documentation if needed

### Modifying the Data Schema

1. Update `ProductKeyManager/DataAccess/DataObjects/ProductKeyDataObject.cs`
2. Update mappings in `ProductKeyManager/Service/Mapping/ProductKeyMappingExtensions.cs`
3. Update domain model in `ProductKeyManager/Service/Models/ProductKey.cs`
4. Update API models if needed
5. Run integration tests to verify persistence

### Adding a New Filter

1. Add the filter property to `ProductKeyManager/Api/Models/GetProductKeyRequest.cs`
2. Implement filtering logic in `ProductKeyManager/Service/ProductKeyService.cs`
3. Add tests for the new filter
4. Update API documentation

## Dependencies

### Core Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| NuciAPI | 3.6.1 | HTTP middleware and request processing |
| NuciAPI.Controllers | 2.3.1 | Controller infrastructure |
| NuciAPI.Middleware | 2.0.3 | Middleware pipeline |
| NuciAPI.Middleware.ExceptionHandling | 1.0.2 | Global exception handling |
| NuciAPI.Middleware.Logging | 1.0.1 | Request logging |
| NuciAPI.Middleware.Security | 1.0.6 | Scanner and replay protection |
| NuciDAL | 3.2.1 | XML data access layer |
| NuciExtensions | 5.3.2 | Collection utilities |
| NuciLog | 1.2.1 | Logging implementation |
| NuciLog.Core | 3.1.0 | Logging interfaces |
| NuciSecurity.HMAC | 4.1.3 | HMAC signing and verification |

### Test Dependencies

| Package | Purpose |
|---------|---------|
| NUnit | Test framework |
| NSubstitute | Mocking framework |
| Microsoft.AspNetCore.Mvc.Testing | Integration test host |
| Microsoft.NET.Test.Sdk | Test SDK |

## Release Process

The project uses `release.sh` for automated releases:

```bash
bash ./release.sh <version>
```

Example:
```bash
bash ./release.sh 5.1.2
```

The script downloads and executes an external release helper from the maintainer's deployment scripts repository.

**Note:** Review the external script before running in your environment.

## Troubleshooting

### Common Issues

1. **Build fails with missing dependencies:**
   ```bash
   dotnet restore ProductKeyManager.slnx
   ```

2. **Tests fail with file access errors:**
   - Ensure the test runner has write permissions to the working directory
   - Check that no other process is locking the XML files

3. **HMAC authentication fails:**
   - Verify the `sharedSecretKey` matches between client and server
   - Check that the request body is not modified after signing

4. **Configuration not loading:**
   - Ensure `appsettings.json` is in the correct location
   - Check that the configuration section names match exactly

### Getting Help

- Check existing issues on GitHub
- Review the architecture documentation in `ARCHITECTURE.md`
- Consult the API documentation in `docs/api/API.md`
- Check the configuration reference in `docs/CONFIGURATION.md`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Ensure all tests pass
5. Update documentation
6. Submit a pull request

### Pull Request Guidelines

- Include a clear description of the changes
- Reference any related issues
- Ensure tests cover new functionality
- Keep changes focused and atomic
- Follow the existing code style