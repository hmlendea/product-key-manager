# Product Key Manager Configuration

This document describes all configuration options for the Product Key Manager service.

## Configuration File

The service reads configuration from `ProductKeyManager/appsettings.json`.

## Settings Reference

### Security Settings

| Key | Type | Default | Required | Description |
|-----|------|---------|----------|-------------|
| `securitySettings.sharedSecretKey` | string | No default | Yes | Shared secret for HMAC API-key authorization. Must be a secure random value. |

**Example:**
```json
{
  "securitySettings": {
    "sharedSecretKey": "your-secure-random-secret-key-here"
  }
}
```

### Data Store Settings

| Key | Type | Default | Required | Description |
|-----|------|---------|----------|-------------|
| `dataStoreSettings.productKeysStorePath` | string | `Data/keys.xml` | Yes | XML file path for persisting product-key records. |

**Example:**
```json
{
  "dataStoreSettings": {
    "productKeysStorePath": "Data/keys.xml"
  }
}
```

### Logging Settings

| Key | Type | Default | Required | Description |
|-----|------|---------|----------|-------------|
| `nuciLoggerSettings.minimumLevel` | string | `Info` | No | Minimum log level (Trace, Debug, Info, Warning, Error, Critical, None) |
| `nuciLoggerSettings.logFilePath` | string | `logfile.log` | No | File path for log output |
| `nuciLoggerSettings.isFileOutputEnabled` | boolean | `true` | No | Enables file logging |

**Example:**
```json
{
  "nuciLoggerSettings": {
    "minimumLevel": "Info",
    "logFilePath": "logfile.log",
    "isFileOutputEnabled": true
  }
}
```

## Complete Example

```json
{
  "securitySettings": {
    "sharedSecretKey": "your-secure-random-secret-key-here"
  },
  "dataStoreSettings": {
    "productKeysStorePath": "Data/keys.xml"
  },
  "nuciLoggerSettings": {
    "minimumLevel": "Info",
    "logFilePath": "logfile.log",
    "isFileOutputEnabled": true
  }
}
```

## Environment Variables

All settings can be overridden using environment variables with the following naming convention:

- `SecuritySettings__SharedSecretKey`
- `DataStoreSettings__ProductKeysStorePath`
- `NuciLoggerSettings__MinimumLevel`
- `NuciLoggerSettings__LogFilePath`
- `NuciLoggerSettings__IsFileOutputEnabled`

## Configuration Reload

The service does not support hot reload of configuration. After modifying `appsettings.json`, restart the service for changes to take effect.

## Security Considerations

1. **Shared Secret:** The `sharedSecretKey` must be kept confidential. Use a strong random value (minimum 32 characters recommended).
2. **File Permissions:** Ensure the XML data file and log files have appropriate file system permissions.
3. **HTTPS:** The service enforces HTTPS redirection. Production deployments should use valid TLS certificates.
4. **Network Exposure:** Limit network exposure using firewalls, reverse proxies, or API gateways.

## File System Requirements

- The application requires write access to the directory containing the XML data file
- The application requires write access to the log file directory
- Default paths are relative to the application working directory