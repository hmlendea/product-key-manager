# Product Key Manager API Documentation

This document describes the HTTP API for the Product Key Manager service.

## Base URL

The API is available at the root path of the service. All endpoints are under `/ProductKeys`.

## Authentication

All API requests must be authenticated using HMAC-based API key authorization. The shared secret is configured in `appsettings.json` under `securitySettings.sharedSecretKey`.

### HMAC Signature

Requests must include an `Authorization` header with the HMAC signature. The signature is computed using the shared secret and the request body.

## Endpoints

### GET /ProductKeys

Retrieve product keys matching the specified filters.

#### Request

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `store` | string | No | - | Filter by store name (supports regex) |
| `product` | string | No | - | Filter by product name (supports regex) |
| `key` | string | No | - | Filter by key value (supports regex) |
| `owner` | string | No | - | Filter by owner identifier (supports regex) |
| `status` | string | No | - | Filter by status (supports regex) |
| `count` | integer | No | 1 | Maximum number of results (1-1000) |

#### Response

**Success (200 OK):**
```json
{
  "products": [
    {
      "store": "string",
      "product": "string",
      "key": "string",
      "owner": "string",
      "comment": "string",
      "status": "string"
    }
  ],
  "count": 0
}
```

**Error Responses:**
- `400 Bad Request` - Invalid request format or validation errors
- `401 Unauthorized` - Invalid or missing HMAC signature
- `500 Internal Server Error` - No matching keys found or server error

#### Example

```bash
curl -X GET "https://localhost/ProductKeys" \
  -H "Authorization: HMAC <signature>" \
  -d '{"store": "MyStore", "count": 10}'
```

### POST /ProductKeys

Add a new product key.

#### Request

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `store` | string | Yes | Store name |
| `product` | string | Yes | Product name |
| `key` | string | Yes | Product key value |
| `owner` | string | Yes | Owner identifier |
| `comment` | string | No | Optional comment |
| `status` | string | No | Status (defaults to "Unknown") |

#### Response

**Success (200 OK):**
```json
{
  "success": true,
  "message": "Product key added successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid request format or validation errors
- `401 Unauthorized` - Invalid or missing HMAC signature
- `500 Internal Server Error` - Server error

#### Example

```bash
curl -X POST "https://localhost/ProductKeys" \
  -H "Authorization: HMAC <signature>" \
  -H "Content-Type: application/json" \
  -d '{
    "store": "MyStore",
    "product": "MyProduct",
    "key": "ABCD-EFGH-IJKL",
    "owner": "user@example.com",
    "comment": "License for user",
    "status": "Vacant"
  }'
```

### PUT /ProductKeys

Update an existing product key.

#### Request

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `store` | string | No | Store name (empty preserves existing) |
| `product` | string | No | Product name (empty preserves existing) |
| `key` | string | Yes | Product key value (identifies the record) |
| `owner` | string | No | Owner identifier (empty preserves existing) |
| `comment` | string | No | Comment (empty preserves existing) |
| `status` | string | No | Status (empty preserves existing) |

#### Response

**Success (200 OK):**
```json
{
  "success": true,
  "message": "Product key updated successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid request format or validation errors
- `401 Unauthorized` - Invalid or missing HMAC signature
- `404 Not Found` - Product key not found
- `500 Internal Server Error` - Server error

#### Example

```bash
curl -X PUT "https://localhost/ProductKeys" \
  -H "Authorization: HMAC <signature>" \
  -H "Content-Type: application/json" \
  -d '{
    "key": "ABCD-EFGH-IJKL",
    "status": "Used",
    "comment": "Activated on 2024-01-15"
  }'
```

## Status Values

The following status values are supported:

| Status | Description |
|--------|-------------|
| `Unknown` | Default status |
| `Used` | Key has been used/activated |
| `Vacant` | Key is available for use |
| `Invalid` | Key is invalid |
| `AlreadyOwned` | Key is already owned by another user |
| `RequiresBaseProduct` | Key requires a base product |
| `RegionLocked` | Key is region-locked |

## Filtering

All string filter fields support regular expressions. The filter is applied as a regex match against the corresponding field.

### Filter Examples

- Exact match: `"store": "MyStore"`
- Prefix match: `"store": "^MyStore"`
- Suffix match: `"store": "Store$"`
- Contains: `"store": "Store"`
- Case-insensitive: Use `(?i)` prefix in regex

## Rate Limiting

No built-in rate limiting is implemented. Operators should implement rate limiting at the infrastructure level (reverse proxy, API gateway, etc.).

## Error Handling

All error responses follow a consistent format:

```json
{
  "success": false,
  "error": "Error description",
  "details": {}
}
```

## Logging

Request metadata is logged for operational purposes. No request bodies or sensitive data are logged.

## Versioning

The API is unversioned. Breaking changes will be communicated through release notes.

## CORS

CORS is not configured by default. Operators should configure CORS policies as needed for their deployment.

## HTTPS

HTTPS redirection is enabled by default. Production deployments should use valid TLS certificates.