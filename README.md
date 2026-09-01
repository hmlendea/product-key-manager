[![Latest Release](https://img.shields.io/github/v/release/hmlendea/product-key-manager)](https://github.com/hmlendea/product-key-manager/releases/latest)
[![Build Status](https://github.com/hmlendea/product-key-manager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/product-key-manager/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/hmlendea/product-key-manager)](https://github.com/hmlendea/product-key-manager/blob/master/LICENSE)

# Product Key Manager

A REST API for securely storing, retrieving, and updating product keys.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Usage](#usage)
- [Configuration](#configuration)
	- [Configuration Files](#configuration-files)
	- [Settings](#settings)
	- [Reload Behaviour](#reload-behaviour)
- [Compatibility](#compatibility)
- [Privacy and Data](#privacy-and-data)
	- [Data Locations](#data-locations)
- [Development](#development)
	- [Requirements](#requirements)
	- [Setup](#setup)
	- [Build](#build)
	- [Run](#run)
	- [Test](#test)
	- [Release](#release)
	- [Dependencies](#dependencies)
- [Project Structure](#project-structure)
	- [Projects and Packages](#projects-and-packages)
	- [Directories](#directories)
- [Architecture](#architecture)
- [Contributing](#contributing)
- [Project Engagement](#project-engagement)
- [License](#license)

## ✨ Capabilities

- Add product keys with store, product, key, owner, comment, and status metadata.
- Retrieve up to 1,000 keys per request, filtered by store, product, key, owner, or status.
- Update existing product-key records.
- Protect API operations with HMAC-based API-key authorisation and replay protection.

## 🚀 Usage

Configure a shared secret, then start the service:

```bash
dotnet run --project ProductKeyManager
```

Send HMAC-authenticated requests to `/ProductKeys`:

| Method | Purpose | Request Body Fields |
|--------|---------|---------------------|
| `GET` | Retrieve matching product keys. | `store`, `product`, `key`, `owner`, `status`, `count` |
| `POST` | Add a product key. | `store`, `product`, `key`, `owner`, `comment`, `status` |
| `PUT` | Update a product key. | `store`, `product`, `key`, `owner`, `comment`, `status` |

The `count` field defaults to `1` and accepts values from `1` to `1000`. String filters support regular expressions.

## ⚙️ Configuration

The service reads its settings from `ProductKeyManager/appsettings.json`.

### Configuration Files

| File | Scope | Purpose |
|------|-------|---------|
| `ProductKeyManager/appsettings.json` | Application | Configures API authorisation, product-key storage, and logging. |

### Settings

The subsequent settings are recognised:
| Section | Key | Type | Default | Required | Description |
|---------|-----|------|---------|----------|-------------|
| `securitySettings` | `sharedSecretKey` | string | No default | Yes | Shared secret for HMAC API-key authorisation. Configure a secure value. |
| `dataStoreSettings` | `productKeysStorePath` | string | `Data/keys.xml` | Yes | XML file that persists product-key records. |
| `nuciLoggerSettings` | `minimumLevel` | string | `Info` | No | Minimum log level. |
| `nuciLoggerSettings` | `logFilePath` | string | `logfile.log` | No | File path for log output. |
| `nuciLoggerSettings` | `isFileOutputEnabled` | boolean | `true` | No | Enables file logging. |

### Reload Behaviour

Restart the service after modifying `appsettings.json`.

## 🧩 Compatibility

| Component | Supported Versions | Notes |
|-----------|--------------------|-------|
| .NET | `10.0` | The application targets `net10.0`. |

## 🛡️ Privacy and Data

| Data | Purpose | Storage | Retention | Optional |
|------|---------|---------|-----------|----------|
| Product-key records, including key, owner, and comment fields | Product-key management | XML file configured by `dataStoreSettings.productKeysStorePath` | No automatic expiry is configured. | No |
| Request logs | Operational logging | File configured by `nuciLoggerSettings.logFilePath` when file output is enabled | Not configured by the application. | Yes |

### Data Locations

| Platform or Scope | Location | Contents |
|-------------------|----------|----------|
| Application working directory | `Data/keys.xml` by default | Persisted product-key records. |
| Application working directory | `logfile.log` by default | File log output when enabled. |

## 🛠️ Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

Restore the solution dependencies:

```bash
dotnet restore ProductKeyManager.slnx
```

### Build

```bash
dotnet build ProductKeyManager.slnx
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
bash ./release.sh 5.1.2
```

This script downloads and executes an external release helper from `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.0.sh`.

**Note:** Piping into `bash` is an intensely controversial topic. Please review any external scripts before running them in your environment!

### Dependencies

| Package | Version | Scope | Purpose |
|---------|---------|-------|---------|
| `NuciAPI` | `3.5.1` | Runtime | API framework types and infrastructure. |
| `NuciAPI.Controllers` | `2.3.1` | Runtime | Controller request processing and authorisation. |
| `NuciAPI.Middleware` | `2.0.2` | Runtime | Middleware infrastructure. |
| `NuciAPI.Middleware.ExceptionHandling` | `1.0.1` | Runtime | Global exception handling. |
| `NuciAPI.Middleware.Logging` | `1.0.1` | Runtime | Request logging. |
| `NuciAPI.Middleware.Security` | `1.0.5` | Runtime | Scanner and replay-attack protection. |
| `NuciDAL` | `3.1.0` | Runtime | XML data persistence. |
| `NuciExtensions` | `5.3.1` | Runtime | Collection and object utilities. |
| `NuciLog` | `1.2.1` | Runtime | Logger implementation. |
| `NuciLog.Core` | `3.0.0` | Runtime | Logging interfaces and types. |
| `NuciSecurity.HMAC` | `4.1.3` | Runtime | HMAC request signing and verification. |

## 🗂️ Project Structure

The solution contains the following projects:

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `ProductKeyManager/ProductKeyManager.csproj` | ASP.NET Core web application | Product-key management API. |
| `ProductKeyManager.UnitTests/ProductKeyManager.UnitTests.csproj` | Unit-test project | Tests for API models and product-key services. |

### Directories

| Directory | Purpose |
|-----------|---------|
| `ProductKeyManager/Api/` | Controllers and API request and response models. |
| `ProductKeyManager/Configuration/` | Application settings classes. |
| `ProductKeyManager/DataAccess/` | XML persistence data objects. |
| `ProductKeyManager/Service/` | Product-key business logic, mappings, and domain models. |
| `ProductKeyManager.UnitTests/` | Unit tests. |

## 🏗️ Architecture

See the [architecture documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Add tests for additional or modified functionality

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/product-key-manager/issues)!

If you find this project useful, consider starring ⭐️ it on GitHub!

## 📄 License

This project is being distributed under the `GNU General Public License v3.0` or later.
See [LICENSE](./LICENSE) for further information.
