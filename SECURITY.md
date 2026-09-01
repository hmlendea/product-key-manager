# Security Policy

This policy describes how to report security vulnerabilities in Product Key Manager and applies to the latest release published through GitHub Releases.

## 📑 Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/product-key-manager/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The subsequent report categories are in scope for this repository:
- HMAC API-key authorisation, request integrity, and replay protection.
- Unauthorised access to, modification of, or disclosure of product-key records through the API or XML data store.

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities exclusively within third-party dependencies, unless the issue is caused by this repository's integration.
- Compromised deployments, exposed shared secrets, or configuration files outside this repository.

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.
