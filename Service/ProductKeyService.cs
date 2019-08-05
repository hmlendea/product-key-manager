using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

using NuciDAL.Repositories;
using NuciExtensions;
using NuciLog.Core;
using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Configuration;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Logging;
using ProductKeyManager.Service.Mapping;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Service
{
    public class ProductKeyService : IProductKeyService
    {
        readonly IRepository<ProductKeyEntity> productKeyRepository;
        readonly IHmacEncoder<GetProductKeyRequest> getRequestEncoder;
        readonly IHmacEncoder<StoreProductKeyRequest> storeRequestEncoder;
        readonly SecuritySettings securitySettings;
        readonly ILogger logger;

        public ProductKeyService(
            IRepository<ProductKeyEntity> productKeyRepository,
            IHmacEncoder<GetProductKeyRequest> getRequestEncoder,
            IHmacEncoder<StoreProductKeyRequest> storeRequestEncoder,
            SecuritySettings securitySettings,
            ILogger logger)
        {
            this.productKeyRepository = productKeyRepository;
            this.getRequestEncoder = getRequestEncoder;
            this.storeRequestEncoder = storeRequestEncoder;
            this.securitySettings = securitySettings;
            this.logger = logger;
        }

        public ProductKey GetProductKey(GetProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos = new List<LogInfo>
            {
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName)
            };

            logger.Info(MyOperation.GetProductKey, OperationStatus.Started, logInfos);

            ValidateGetRequest(request);

            ProductKey productKey = FindProductKey(request);

            if (productKey is null)
            {
                logger.Info(MyOperation.GetProductKey, OperationStatus.Failure, logInfos);
                return null;
            }
            
            logger.Info(MyOperation.GetProductKey, OperationStatus.Success, logInfos);
            return productKey;
        }

        public void StoreProductKey(StoreProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos = new List<LogInfo>
            {
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                new LogInfo(MyLogInfoKey.Key, request.Key)
            };

            logger.Info(MyOperation.StoreProductKey, OperationStatus.Started, logInfos);

            ValidateStoreRequest(request);
            ProductKey productKey = CreateProductKeyFromRequest(request);

            StoreProductKey(productKey);
            logger.Debug(MyOperation.StoreProductKey, OperationStatus.Success, logInfos);
        }
        
        void ValidateGetRequest(GetProductKeyRequest request)
        {
            bool isTokenValid = getRequestEncoder.IsTokenValid(request.HmacToken, request, securitySettings.SharedSecretKey);

            if (!isTokenValid && securitySettings.IsEnabled)
            {
                AuthenticationException ex = new AuthenticationException("The provided HMAC token is not valid");

                logger.Error(
                    MyOperation.GetProductKey,
                    OperationStatus.Failure,
                    ex,
                    new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName));

                throw ex;
            }
        }
        
        void ValidateStoreRequest(StoreProductKeyRequest request)
        {
            bool isTokenValid = storeRequestEncoder.IsTokenValid(request.HmacToken, request, securitySettings.SharedSecretKey);

            Exception exception = null;

            if (!isTokenValid && securitySettings.IsEnabled)
            {
                exception = new AuthenticationException("The provided HMAC token is not valid");
            }
            else if (string.IsNullOrWhiteSpace(request.Key))
            {
                exception = new ArgumentNullException("key");
            }
            else if (DoesKeyAlreadyExist(request.Key))
            {
                exception = new ArgumentException("The specified product key already exists");
            }

            if (!(exception is null))
            {
                logger.Error(
                    MyOperation.GetProductKey,
                    OperationStatus.Failure,
                    exception,
                    new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                    new LogInfo(MyLogInfoKey.Key, request.Key));

                throw exception;
            }
        }

        ProductKey FindProductKey(GetProductKeyRequest request)
        {
            IEnumerable<ProductKeyEntity> productKeyCandidates = productKeyRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.StoreName))
            {
                productKeyCandidates = productKeyCandidates.Where(
                    x => x.StoreName.Equals(request.StoreName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                productKeyCandidates = productKeyCandidates.Where(
                    x => x.ProductName.Equals( request.ProductName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (productKeyCandidates.Any())
            {
                return productKeyCandidates.GetRandomElement().ToServiceModel();
            }

            return null;
        }

        ProductKey CreateProductKeyFromRequest(StoreProductKeyRequest request)
        {
            ProductKey productKey = new ProductKey();
            productKey.Id = Guid.NewGuid().ToString();
            productKey.StoreName = request.StoreName;
            productKey.ProductName = request.ProductName;
            productKey.Key = request.Key;
            productKey.AddedDateTime = DateTime.Now;
            productKey.UpdatedDateTime = productKey.AddedDateTime;

            return productKey;
        }

        bool DoesKeyAlreadyExist(string key)
        {
            IEnumerable<ProductKeyEntity> productKeys = productKeyRepository.GetAll();

            if (productKeys.Any(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        void StoreProductKey(ProductKey productKey)
        {
            productKeyRepository.Add(productKey.ToDataObject());
            productKeyRepository.ApplyChanges();
        }
    }
}
