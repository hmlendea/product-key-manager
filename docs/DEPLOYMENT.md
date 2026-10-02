# Product Key Manager Deployment Guide

This document describes how to deploy and operate the Product Key Manager service in production environments.

## Deployment Options

### Direct Deployment

Deploy the compiled application directly to a server:

```bash
# Build for production
dotnet publish ProductKeyManager/ProductKeyManager.csproj -c Release -o ./publish

# Copy to server
scp -r ./publish/* user@server:/opt/product-key-manager/

# Run on server
cd /opt/product-key-manager
dotnet ProductKeyManager.dll
```

### Docker Deployment

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ProductKeyManager/ProductKeyManager.csproj", "ProductKeyManager/"]
RUN dotnet restore "ProductKeyManager/ProductKeyManager.csproj"
COPY . .
WORKDIR "/src/ProductKeyManager"
RUN dotnet build "ProductKeyManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductKeyManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductKeyManager.dll"]
```

Build and run:
```bash
docker build -t product-key-manager .
docker run -d -p 8080:8080 -p 8081:8081 \
  -v /host/data:/app/Data \
  -v /host/logs:/app/logs \
  -e SecuritySettings__SharedSecretKey="your-secret" \
  product-key-manager
```

### Kubernetes Deployment

Example `deployment.yaml`:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: product-key-manager
spec:
  replicas: 2
  selector:
    matchLabels:
      app: product-key-manager
  template:
    metadata:
      labels:
        app: product-key-manager
    spec:
      containers:
      - name: product-key-manager
        image: product-key-manager:latest
        ports:
        - containerPort: 8080
        - containerPort: 8081
        env:
        - name: SecuritySettings__SharedSecretKey
          valueFrom:
            secretKeyRef:
              name: product-key-manager-secrets
              key: shared-secret
        - name: DataStoreSettings__ProductKeysStorePath
          value: "/data/keys.xml"
        - name: NuciLoggerSettings__LogFilePath
          value: "/logs/logfile.log"
        volumeMounts:
        - name: data
          mountPath: /data
        - name: logs
          mountPath: /logs
      volumes:
      - name: data
        persistentVolumeClaim:
          claimName: product-key-manager-data
      - name: logs
        persistentVolumeClaim:
          claimName: product-key-manager-logs
---
apiVersion: v1
kind: Service
metadata:
  name: product-key-manager
spec:
  selector:
    app: product-key-manager
  ports:
  - port: 443
    targetPort: 8081
  type: ClusterIP
```

## Configuration for Production

### Required Settings

1. **Shared Secret:** Generate a strong random key (32+ characters)
2. **Data Store Path:** Use a persistent volume
3. **Log Path:** Use a persistent volume or log aggregation
4. **HTTPS:** Configure valid TLS certificates

### Environment Variables

```bash
export SecuritySettings__SharedSecretKey="your-32-char-minimum-secret-key"
export DataStoreSettings__ProductKeysStorePath="/data/keys.xml"
export NuciLoggerSettings__LogFilePath="/logs/logfile.log"
export NuciLoggerSettings__MinimumLevel="Info"
export NuciLoggerSettings__IsFileOutputEnabled="true"
export ASPNETCORE_URLS="https://+:8081;http://+:8080"
export ASPNETCORE_Kestrel__Certificates__Default__Path="/certs/aspnetapp.pfx"
export ASPNETCORE_Kestrel__Certificates__Default__Password="cert-password"
```

### appsettings.Production.json

```json
{
  "securitySettings": {
    "sharedSecretKey": "[[FROM_ENV]]"
  },
  "dataStoreSettings": {
    "productKeysStorePath": "/data/keys.xml"
  },
  "nuciLoggerSettings": {
    "minimumLevel": "Info",
    "logFilePath": "/logs/logfile.log",
    "isFileOutputEnabled": true
  },
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://+:8081",
        "Certificate": {
          "Path": "/certs/aspnetapp.pfx",
          "Password": "[[FROM_ENV]]"
        }
      },
      "Http": {
        "Url": "http://+:8080"
      }
    }
  }
}
```

## TLS Configuration

### Generate Certificate

```bash
# Self-signed for testing (NOT for production)
dotnet dev-certs https -ep /tmp/aspnetapp.pfx -p "cert-password"

# Production: Use Let's Encrypt, cert-manager, or your CA
```

### Configure Kestrel

The service uses ASP.NET Core's built-in Kestrel server. Configure HTTPS in `appsettings.json` or via environment variables.

## Reverse Proxy

### Nginx Configuration

```nginx
server {
    listen 443 ssl http2;
    server_name api.example.com;

    ssl_certificate /etc/letsencrypt/live/api.example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.example.com/privkey.pem;

    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}

server {
    listen 80;
    server_name api.example.com;
    return 301 https://$server_name$request_uri;
}
```

### HAProxy Configuration

```
frontend https_front
    bind *:443 ssl crt /etc/haproxy/certs/api.example.com.pem
    default_backend product_key_manager

backend product_key_manager
    server pk1 localhost:8080 check
```

## File System Requirements

### Directory Structure

```
/opt/product-key-manager/
├── ProductKeyManager.dll
├── appsettings.json
├── appsettings.Production.json
├── Data/
│   └── keys.xml          # Must be writable
└── logs/
    └── logfile.log       # Must be writable
```

### Permissions

```bash
# Create directories
mkdir -p /opt/product-key-manager/Data /opt/product-key-manager/logs

# Set ownership
chown -R appuser:appgroup /opt/product-key-manager

# Set permissions
chmod 750 /opt/product-key-manager/Data
chmod 750 /opt/product-key-manager/logs
chmod 640 /opt/product-key-manager/Data/keys.xml
chmod 640 /opt/product-key-manager/logs/logfile.log
```

## Monitoring and Observability

### Health Checks

The service does not include built-in health checks. Add a health check endpoint if needed:

```csharp
// In Startup.cs ConfigureServices
services.AddHealthChecks()
    .AddCheck("xml-store", () => 
        File.Exists(dataStoreSettings.ProductKeysStorePath) 
            ? HealthCheckResult.Healthy() 
            : HealthCheckResult.Unhealthy("XML store not found"));
```

### Logging

Logs are written to the configured file in structured JSON format. Integrate with:
- ELK Stack (Elasticsearch, Logstash, Kibana)
- Grafana Loki
- Splunk
- Datadog
- Azure Monitor / AWS CloudWatch

### Metrics

No built-in metrics. Consider adding:
- Prometheus metrics endpoint
- OpenTelemetry integration
- Custom metrics for key operations

## Backup and Recovery

### Backup Strategy

1. **XML Data File:** Backup `Data/keys.xml` regularly
2. **Configuration:** Backup `appsettings.json` and secrets
3. **Logs:** Archive logs per retention policy

### Backup Script Example

```bash
#!/bin/bash
BACKUP_DIR="/backups/product-key-manager/$(date +%Y%m%d)"
mkdir -p "$BACKUP_DIR"

# Backup data
cp /opt/product-key-manager/Data/keys.xml "$BACKUP_DIR/"

# Backup config (excluding secrets)
cp /opt/product-key-manager/appsettings.json "$BACKUP_DIR/"

# Compress
tar -czf "$BACKUP_DIR.tar.gz" -C "$BACKUP_DIR" .
rm -rf "$BACKUP_DIR"

# Retain 30 days
find /backups/product-key-manager -name "*.tar.gz" -mtime +30 -delete
```

### Recovery Procedure

1. Restore `keys.xml` to the data directory
2. Restore configuration
3. Restart the service
4. Verify data integrity via API

## Scaling Considerations

### Horizontal Scaling

The service uses file-based persistence, which limits horizontal scaling:

- **Single Instance:** Recommended for simplicity
- **Multiple Instances:** Requires shared file system (NFS, SMB) or external storage
- **Concurrent Writes:** XML file writes are not thread-safe across processes

### Vertical Scaling

- Increase CPU for HMAC operations
- Increase memory for large datasets (1000+ keys)
- SSD storage for XML file I/O

### Alternative for Scale

For high-scale deployments, consider:
- Migrating to a database (SQL Server, PostgreSQL)
- Implementing a caching layer (Redis)
- Using a message queue for async operations

## Security Hardening

### Network

- Restrict access to trusted networks only
- Use VPN or private networks
- Implement rate limiting at reverse proxy
- Block direct access to management ports

### Application

- Use strong shared secret (rotate periodically)
- Keep .NET runtime updated
- Monitor for security advisories in dependencies
- Implement audit logging for sensitive operations

### File System

- Encrypt data volumes at rest
- Restrict file permissions to service account only
- Monitor file access logs
- Implement integrity checking for XML file

## Updates and Maintenance

### Update Procedure

1. **Schedule maintenance window**
2. **Backup current deployment**
3. **Deploy new version**
4. **Run smoke tests**
5. **Monitor logs for errors**
6. **Rollback if issues detected**

### Rolling Updates (Kubernetes)

```bash
kubectl set image deployment/product-key-manager \
  product-key-manager=product-key-manager:v2.0.0
kubectl rollout status deployment/product-key-manager
```

### Dependency Updates

```bash
# Check for updates
dotnet list ProductKeyManager/ProductKeyManager.csproj package --outdated

# Update packages
dotnet add ProductKeyManager/ProductKeyManager.csproj package NuciAPI --version 3.7.0
```

## Troubleshooting

### Common Issues

| Issue | Cause | Resolution |
|-------|-------|------------|
| Service won't start | Missing shared secret | Set `SecuritySettings__SharedSecretKey` |
| 401 Unauthorized | Invalid HMAC signature | Verify client/server secret match |
| 500 on GET | No matching keys | Expected behavior when no keys match |
| File permission errors | Wrong ownership | Fix directory/file permissions |
| TLS errors | Invalid certificate | Verify cert path, password, format |

### Log Analysis

Check `logfile.log` for:
- `OperationStatus.Failure` entries
- Exception stack traces
- HMAC validation failures
- File I/O errors

### Debug Mode

Enable debug logging temporarily:
```json
{
  "nuciLoggerSettings": {
    "minimumLevel": "Debug"
  }
}
```

## Disaster Recovery

### RPO/RTO Targets

- **RPO (Recovery Point Objective):** 24 hours (daily backups)
- **RTO (Recovery Time Objective):** 1 hour (manual restore)

### Recovery Steps

1. Provision new infrastructure
2. Deploy application
3. Restore `keys.xml` from latest backup
4. Restore configuration
5. Start service
6. Verify API functionality
7. Update DNS/load balancer

## Compliance

### Data Protection

- No personal data beyond what users provide in product keys
- No automated data collection or telemetry
- Operator controls all data retention and deletion
- Supports data subject requests via manual operations

### Audit Requirements

- All API operations are logged
- Logs include timestamps, operation type, and request metadata
- No sensitive data in logs
- Logs retained per operator policy

## Support

### Escalation Path

1. Check logs and monitoring
2. Consult this documentation
3. Check GitHub issues
4. Contact maintainers via GitHub Security Advisories for security issues
5. Open GitHub issue for bugs/features

### Useful Commands

```bash
# Check service status
systemctl status product-key-manager

# View logs
journalctl -u product-key-manager -f

# Test API locally
curl -k https://localhost:8081/ProductKeys \
  -H "Authorization: HMAC <signature>" \
  -d '{"count": 1}'

# Verify XML file
cat /opt/product-key-manager/Data/keys.xml | xmllint --format -
```