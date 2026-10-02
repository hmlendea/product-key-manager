# Product Key Manager Documentation

Welcome to the Product Key Manager documentation. This folder contains comprehensive documentation for developers, operators, and contributors.

## Documentation Index

### Getting Started
- **[README.md](../README.md)** - Project overview, quick start, and usage examples
- **[DEVELOPMENT.md](DEVELOPMENT.md)** - Development setup, building, testing, and contribution guidelines

### Architecture & Design
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - System architecture, components, data flows, and design decisions
- **[ARCHITECTURE.md (root)](../ARCHITECTURE.md)** - Original architecture document

### API Reference
- **[API.md](api/API.md)** - Complete HTTP API documentation with endpoints, authentication, and examples

### Configuration & Operations
- **[CONFIGURATION.md](CONFIGURATION.md)** - All configuration options with examples
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Production deployment guide for various platforms

### Testing
- **[TESTING.md](TESTING.md)** - Testing strategy, test organization, and how to write/extend tests

### Policies
- **[PRIVACY.md](../PRIVACY.md)** - Data handling and privacy practices
- **[SECURITY.md](../SECURITY.md)** - Security vulnerability reporting policy
- **[LICENSE](../LICENSE)** - Project license

## Quick Links

### For Developers
1. Start with [DEVELOPMENT.md](DEVELOPMENT.md) for setup
2. Review [ARCHITECTURE.md](ARCHITECTURE.md) for system understanding
3. Check [API.md](api/API.md) for API contracts
4. Read [TESTING.md](TESTING.md) for testing practices

### For Operators
1. Review [CONFIGURATION.md](CONFIGURATION.md) for all settings
2. Follow [DEPLOYMENT.md](DEPLOYMENT.md) for production deployment
3. Check [PRIVACY.md](../PRIVACY.md) for data handling
4. Review [SECURITY.md](../SECURITY.md) for security practices

### For Contributors
1. Read [DEVELOPMENT.md](DEVELOPMENT.md) for contribution workflow
2. Understand [ARCHITECTURE.md](ARCHITECTURE.md) for design constraints
3. Follow [TESTING.md](TESTING.md) for test requirements
4. Check [SECURITY.md](../SECURITY.md) for vulnerability reporting

## Project Structure

```
product-key-manager/
├── docs/
│   ├── api/
│   │   └── API.md              # API documentation
│   ├── ARCHITECTURE.md         # Architecture documentation
│   ├── CONFIGURATION.md        # Configuration reference
│   ├── DEPLOYMENT.md           # Deployment guide
│   ├── DEVELOPMENT.md          # Development guide
│   └── TESTING.md              # Testing guide
├── ProductKeyManager/          # Main application
├── ProductKeyManager.UnitTests/     # Unit tests
├── ProductKeyManager.IntegrationTests/ # Integration tests
├── ARCHITECTURE.md             # Architecture (root copy)
├── PRIVACY.md                  # Privacy policy
├── SECURITY.md                 # Security policy
├── README.md                   # Project overview
└── ProductKeyManager.slnx      # Solution file
```

## Key Concepts

### Product Key Manager
A REST API for securely storing, retrieving, and updating product keys with HMAC-based authentication.

### Core Capabilities
- Add product keys with metadata (store, product, key, owner, comment, status)
- Retrieve keys with flexible filtering (regex support) and pagination
- Update existing key records
- HMAC-signed requests and responses
- Replay attack protection
- XML file persistence

### Technology Stack
- **.NET 10.0** - Target framework
- **ASP.NET Core** - Web framework
- **NuciAPI** - HTTP middleware, auth, logging
- **NuciDAL** - XML data access
- **NuciLog** - Structured logging
- **NuciSecurity.HMAC** - HMAC signing

## Support

- **Issues:** [GitHub Issues](https://github.com/hmlendea/product-key-manager/issues)
- **Security:** [Security Advisories](https://github.com/hmlendea/product-key-manager/security/advisories)
- **Releases:** [GitHub Releases](https://github.com/hmlendea/product-key-manager/releases)

## License

This project is licensed under the terms specified in [LICENSE](../LICENSE).