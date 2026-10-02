# Privacy and Personal Data

This document describes how Product Key Manager at https://github.com/hmlendea/product-key-manager handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

**Information reviewed:** 2026-10-02

## 📑 Table of Contents

- What This Document Covers
- Self-Hosted Deployments
- Data We Handle
- Processing and Use
- Storage, Retention, and Deletion
- External Processing and Integrations
- Data Protection and Security
- Document Changes
- Contact

## 🔎 What This Document Covers

This document describes how Product Key Manager at https://github.com/hmlendea/product-key-manager handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 🏠 Self-Hosted Deployments

Product Key Manager is a self-hosted ASP.NET Core web application. This document covers the application behaviour as distributed by the project maintainers. Self-hosted instance operators control their instance's configuration, local storage, logs, backups, access controls, retention, and request handling. The project maintainers do not operate user instances and do not receive data from self-hosted deployments.

The application does not send telemetry, crash reports, update checks, or any other data to project maintainers or external services. No optional data flows to external parties exist in the distributed software.

## 📥 Data We Handle

### Data Provided to the Application

- Product-key records submitted via the API, including store name, product name, key value, owner identifier, comment, status, and a generated confirmation code.

### Data Generated or Collected by the Application

- Request logs written by the NuciLog logging framework when file output is enabled, containing request metadata such as timestamps, HTTP method, path, and HMAC verification results. No request bodies or sensitive fields are logged.

### Data Received from Integrations

- No personal data is received from integrations or third parties.

## 🧭 Processing and Use

The application processes the data described above for these verified functions:
- Product-key retrieval (GET /ProductKeys) — Product-key records and request filters
- Product-key addition (POST /ProductKeys) — Product-key records
- Product-key update (PUT /ProductKeys) — Product-key records
- HMAC-based API authorisation and replay protection — Shared secret key and request headers
- Operational logging — Request metadata when file logging is enabled

## 🗄️ Storage, Retention, and Deletion

Product-key records are stored in an XML file at the path configured by `dataStoreSettings.productKeysStorePath` (default `Data/keys.xml`). Request logs are written to the file configured by `nuciLoggerSettings.logFilePath` (default `logfile.log`) when `nuciLoggerSettings.isFileOutputEnabled` is `true`.

The application does not implement automatic expiry or deletion for product-key records or log files. The instance operator controls storage location, backups, retention, and deletion for both data categories.

## 🔗 External Processing and Integrations

The application has no built-in external data transfers. No external services, recipients, or integrations process or receive data from the application.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| None | — | — | — |

## 🛡️ Data Protection and Security

The application enforces HMAC-based API-key authorisation using a shared secret configured in `securitySettings.sharedSecretKey`. Replay protection is applied via the NuciAPI middleware. HTTPS redirection is enabled by default. The instance operator is responsible for securing the shared secret, managing file-system permissions for the XML store and log files, configuring network exposure, applying updates, and protecting backups. No absolute security is guaranteed.

## 🔄 Document Changes

Update this document when application data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/product-key-manager/blob/master/PRIVACY.md.

## 📬 Contact

For questions about application data handling, contact the project maintainers via GitHub issues at https://github.com/hmlendea/product-key-manager/issues. For a self-hosted instance, contact the instance operator, unless the project explicitly handles the request. Include the deployment identifier or relevant configuration details if applicable; do not send passwords, access tokens, or other secrets.