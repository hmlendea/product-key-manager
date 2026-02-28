[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Latest GitHub release](https://img.shields.io/github/v/release/hmlendea/product-key-manager)](https://github.com/hmlendea/product-key-manager/releases/latest) [![Build Status](https://github.com/hmlendea/product-key-manager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/product-key-manager/actions/workflows/dotnet.yml)
# 🔐 Product Key Manager

> A lightweight RESTful service for managing product keys – storing, filtering, and retrieving them securely via API.

---

## 🚀 Features

- Store and retrieve product keys via HTTP GET/POST
- Filtering by store, product, owner, key status, etc.
- HMAC verification for secure access
- Clean architecture with DI, service layers and configuration

---

## 📦 API Overview

### `GET /ProductKeys`

Retrieve product keys by query parameters:

#### Query Parameters:

| Param   | Type   | Description                         |
|---------|--------|-------------------------------------|
| store   | string | Key store identifier                |
| product | string | Product name                        |
| key     | string | Product key itself                  |
| owner   | string | Key owner                           |
| status  | string | Key status (`Vacant`, `Used`, etc.) |
| count   | int    | Number of results to return         |
| hmac    | string | Optional signature for verification |

---

## 🧪 Running Locally

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/)

### Run via CLI

```bash
git clone https://github.com/hmlendea/product-key-manager.git
cd product-key-manager
dotnet run
```

App will start on: `http://localhost:5000`

---

## ⚙️ Configuration

Default config is in `appsettings.json`. You can override via environment variables or secrets:

```json
{
  "securitySettings": {
    "sharedSecretKey": "[[PRODUCT_KEY_MANAGER_SSK]]"
  },
  "dataStoreSettings": {
    "productKeysStorePath": "[[PATH_TO_KEYS_XML]]"
  },
  "nuciLoggerSettings": {
    "minimumLevel": "Info",
    "logFilePath": "logfile.log",
    "isFileOutputEnabled": true
  }
}
```

---

## 🛡️ Security

API requests are secured using HMAC validation:

1. Set the HMAC key/secret in config
2. Send HMAC as query param
3. Service will verify authenticity

---

## 💖 Support

If this project helps you, consider [donating](https://hmlendea.go.ro/fund.html) or giving a ⭐️ on GitHub!

---

## 📄 License

[GPL-3.0](https://github.com/hmlendea/product-key-manager/blob/master/LICENSE) © [hmlendea](https://github.com/hmlendea)
