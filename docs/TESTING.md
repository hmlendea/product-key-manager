# Product Key Manager Testing Guide

This document describes the testing strategy, test organization, and how to run and extend tests for the Product Key Manager project.

## Test Strategy

The project uses a multi-layered testing approach:

1. **Unit Tests** - Test individual components in isolation with mocked dependencies
2. **Integration Tests** - Test HTTP API endpoints, authentication, and persistence
3. **End-to-End Tests** - Test complete workflows (covered by integration tests)

## Test Organization

### Unit Tests (`ProductKeyManager.UnitTests/`)

```
ProductKeyManager.UnitTests/
├── Service/
│   └── ProductKeyServiceTests.cs      # Service logic tests
└── Api/
    └── Models/
        └── GetProductKeyResponseTests.cs  # API model tests
```

**Characteristics:**
- Fast execution (< 1 second)
- No external dependencies
- Use NSubstitute for mocking
- Test business logic, filtering, mapping, error handling

### Integration Tests (`ProductKeyManager.IntegrationTests/`)

```
ProductKeyManager.IntegrationTests/
├── ProductKeysApiIntegrationTests.cs           # CRUD and filtering tests
├── ProductKeysApiSecurityIntegrationTests.cs   # Security tests
├── ProductKeysApiPersistenceIntegrationTests.cs # Persistence tests
├── ProductKeyXmlPersistenceIntegrationTests.cs # Repository tests
├── Assertions/
│   └── ProductKeyApiResponseAssertions.cs      # Response assertions
├── Infrastructure/
│   ├── ProductKeyManagerApiFactory.cs          # Test host factory
│   ├── ProductKeyManagerApiTestBase.cs         # Base test class
│   └── ProductKeyRequestFactory.cs             # Request factory
└── ProductKeyManagerApiTestBase.cs             # Shared test base
```

**Characteristics:**
- Test full HTTP pipeline
- Use in-memory XML stores for isolation
- Test authentication, validation, persistence
- Run against real ASP.NET Core middleware

## Running Tests

### All Tests

```bash
dotnet test ProductKeyManager.slnx
```

### Unit Tests Only

```bash
dotnet test ProductKeyManager.UnitTests/ProductKeyManager.UnitTests.csproj
```

### Integration Tests Only

```bash
dotnet test ProductKeyManager.IntegrationTests/ProductKeyManager.IntegrationTests.csproj
```

### With Coverage

```bash
dotnet test ProductKeyManager.slnx --collect:"XPlat Code Coverage"
```

### Specific Test Filter

```bash
# Run specific test class
dotnet test --filter "FullyQualifiedName~ProductKeyServiceTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~GivenRepositoryHasOneMatchingEntity_WhenGetProductKeyIsCalled_ThenReturnsResponseWithOneKey"

# Run tests by category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

## Test Infrastructure

### ProductKeyManagerApiFactory

Creates an isolated test host with:
- Unique XML store path per test run
- Configured shared secret for HMAC
- Disabled file logging
- HTTPS client with proper headers

### ProductKeyManagerApiTestBase

Base class providing:
- `Client` - HTTP client for API requests
- `AddProductKeyAsync()` - Helper to add test data
- Common setup and teardown

### ProductKeyRequestFactory

Creates properly signed HTTP requests:
- `CreateGetRequest()` - GET request with HMAC
- `CreateAddRequest()` - POST request with HMAC
- `CreateUpdateRequest()` - PUT request with HMAC

### ProductKeyApiResponseAssertions

Assertion helpers for API responses:
- `AssertSuccessfulResponseAsync()` - Validates 200 OK with expected count
- `AssertMutationSucceededAsync()` - Validates successful mutation
- `AssertStatusAsync()` - Validates specific HTTP status code

## Writing Unit Tests

### Test Class Structure

```csharp
[TestFixture]
public sealed class MyComponentTests
{
    private IMyDependency _dependency;
    private MyComponent _sut;

    [SetUp]
    public void SetUp()
    {
        _dependency = Substitute.For<IMyDependency>();
        _sut = new MyComponent(_dependency);
    }

    [Test]
    public void GivenCondition_WhenAction_ThenExpectedResult()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

### Naming Convention

Test methods follow the pattern:
```
Given[Precondition]_When[Action]_Then[ExpectedOutcome]
```

Examples:
- `GivenRepositoryHasOneMatchingEntity_WhenGetProductKeyIsCalled_ThenReturnsResponseWithOneKey`
- `GivenRepositoryIsEmpty_WhenGetProductKeyIsCalled_ThenThrowsNullReferenceException`

### Assertions

Use NUnit's `Assert.That()` with constraints:
```csharp
Assert.That(result, Is.Not.Null);
Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
Assert.Throws<NullReferenceException>(() => service.GetProductKey(request));
```

Use `Assert.Multiple()` for multiple assertions:
```csharp
Assert.Multiple(() =>
{
    Assert.That(product.GetProperty("store").GetString(), Is.EqualTo("MyStore"));
    Assert.That(product.GetProperty("status").GetString(), Is.EqualTo("Vacant"));
});
```

## Writing Integration Tests

### Test Class Structure

```csharp
[TestFixture]
[NonParallelizable]
public sealed class MyApiIntegrationTests : ProductKeyManagerApiTestBase
{
    [Test]
    public async Task GivenCondition_WhenAction_ThenExpectedResult()
    {
        // Arrange - use helpers from base class
        await AddProductKeyAsync("Store", "Product", "KEY-001", "Owner", "Comment", "Vacant");

        // Act - use request factory
        using HttpResponseMessage response = await Client.SendAsync(
            ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest { Key = "KEY-001" }));

        // Assert - use assertion helpers
        using JsonDocument result = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(response, 1);
    }
}
```

### Test Isolation

Each test gets a fresh XML store via the factory. Tests should not depend on shared state.

### Async Tests

All integration tests are async and use `await` for HTTP calls.

## Test Data

### Product Key Status Values

Tests should cover all status values:
- `Unknown` (default)
- `Used`
- `Vacant`
- `Invalid`
- `AlreadyOwned`
- `RequiresBaseProduct`
- `RegionLocked`
- `null` / empty string (treated as Unknown)

### Filter Test Cases

Test regex filtering with:
- Exact matches
- Prefix matches (`^prefix`)
- Suffix matches (`suffix$`)
- Contains matches
- Case-insensitive matches (`(?i)pattern`)
- No matches (should return 500 for GET, 404 for PUT)

### Boundary Tests

Test count boundaries:
- `count = 1` (minimum)
- `count = 1000` (maximum)
- `count > available records` (returns all available)
- Multiple records with ordering

## Security Testing

### HMAC Authentication

Tests verify:
- Valid signatures succeed
- Invalid signatures return 401
- Missing signatures return 401
- Replay protection works
- Timestamp validation works

### Input Validation

Tests verify:
- Required fields are validated
- Count range (1-1000) is enforced
- Status values are validated
- Empty/null handling

## Persistence Testing

### XML Store Tests

Tests verify:
- File creation on first run
- Data persistence across restarts
- Concurrent read/write safety
- Schema compatibility
- Empty file handling

### Data Integrity

Tests verify:
- All fields round-trip correctly
- Timestamps are preserved
- Confirmation codes are generated
- Updates preserve unchanged fields

## Continuous Integration

Tests run automatically on:
- Pull requests
- Pushes to master
- Release tags

### CI Configuration

See `.github/workflows/dotnet.yml` for the CI pipeline configuration.

## Troubleshooting Tests

### Common Issues

1. **Tests fail with "Access denied" on XML files:**
   - Ensure test runner has write permissions
   - Check for file locks from previous test runs

2. **Integration tests fail with "Connection refused":**
   - Verify the test host starts correctly
   - Check for port conflicts

3. **HMAC validation fails in tests:**
   - Verify the test factory uses the correct shared secret
   - Check that request factory signs requests properly

4. **Tests are flaky:**
   - Ensure proper test isolation (unique store paths)
   - Check for shared state between tests
   - Verify async/await usage is correct

### Debugging Tests

#### In VS Code

1. Set breakpoints in test code
2. Use "Debug Test" from the test explorer
3. Inspect variables during test execution

#### In Visual Studio

1. Open Test Explorer
2. Right-click test → Debug
3. Use immediate window for inspection

## Extending Tests

### Adding New Unit Tests

1. Identify the component to test
2. Create test class in appropriate folder
3. Mock dependencies with NSubstitute
4. Follow naming convention
5. Cover happy path, edge cases, and error conditions

### Adding New Integration Tests

1. Determine if it's CRUD, security, or persistence
2. Add to appropriate test class or create new one
3. Use test infrastructure helpers
4. Follow async patterns
5. Clean up test data (automatic via isolated stores)

### Adding Test Categories

Use `[Category("CategoryName")]` attribute:
```csharp
[Test]
[Category("Performance")]
public async Task GivenLargeDataset_WhenFiltering_ThenCompletesWithinTimeLimit()
```

Run with:
```bash
dotnet test --filter "Category=Performance"
```

## Test Maintenance

### Regular Tasks

- Update tests when API contracts change
- Add tests for new features
- Remove obsolete tests
- Monitor test execution time
- Keep test dependencies updated

### Test Quality

- Tests should be deterministic
- Tests should be independent
- Tests should be fast (unit tests < 100ms each)
- Tests should have clear failure messages
- Tests should cover both success and failure paths