[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/product-key-manager)](https://github.com/hmlendea/product-key-manager/releases/latest)
[![Build Status](https://github.com/hmlendea/product-key-manager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/product-key-manager/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# Product Key Manager

A lightweight RESTful service for managing product keys - storing, filtering, and retrieving them securely via API.

## Features

- Store, retrieve, and update product keys via HTTP GET/POST/PUT
- Filter keys by store, product, owner, key value, and status
- String filters support regex patterns
- HMAC request signing for secure access
- Product key statuses: `Unknown`, `Vacant`, `Used`, `Invalid`, `AlreadyOwned`, `RequiresBaseProduct`, `RegionLocked`

## Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)

All NuGet dependencies are restored automatically by `dotnet restore`.

### Build

```bash
dotnet build ProductKeyManager
```

### Run

```bash
dotnet run --project ProductKeyManager
```

### Test

```bash
dotnet test ProductKeyManager.slnx
```

### Release

The repository includes `release.sh`, which delegates to the upstream deployment script used by the project maintainer.

```bash
bash ./release.sh 1.0.0
```

This script downloads and executes an external release helper from `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.0.sh`.

**Note:** Piping into `bash` is an intensely controversial topic. Please review any external scripts before running them in your environment!

## Project Structure

The solution contains the following projects:

- `ProductKeyManager` - main API application
- `ProductKeyManager.UnitTests` - unit tests

Key directories inside `ProductKeyManager/`:

| Directory | Purpose |
|-----------|---------|
| `Api/Controllers/` | HTTP controllers |
| `Api/Models/` | Request and response DTOs |
| `Configuration/` | Settings classes |
| `DataAccess/DataObjects/` | XML data objects |
| `Logging/` | Log operation and info-key definitions |
| `Service/` | Business logic services |
| `Service/Mapping/` | Domain model mapping extensions |
| `Service/Models/` | Domain models |

### Dependencies

| Package | Purpose |
|---------|---------|
| NuciAPI | Base API framework types and infrastructure |
| NuciAPI.Controllers | Base controller with HMAC-validated request processing |
| NuciAPI.Middleware | Core middleware support |
| NuciAPI.Middleware.ExceptionHandling | Global exception-handling middleware |
| NuciAPI.Middleware.Logging | Request-logging middleware |
| NuciAPI.Middleware.Security | Scanner protection and replay-attack middleware |
| NuciDAL | XML file repository for data persistence |
| NuciExtensions | Collection and object extension utilities |
| NuciLog | Logger implementation |
| NuciLog.Core | Logging interfaces and types |
| NuciSecurity.HMAC | HMAC request signing and verification |

## Contributing

Contributions are welcome.

Please:

- keep changes cross-platform
- keep pull requests focused and consistent with existing style
- update documentation when behaviour changes
- add or update tests for new behaviour

## Support

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or giving a ⭐️ on GitHub!

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](./LICENSE) for details.
