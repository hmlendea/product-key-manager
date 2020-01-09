using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

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
        readonly IHmacEncoder<UpdateProductKeyRequest> updateRequestEncoder;
        readonly IHmacEncoder<ProductKeyResponse> productKeyResponseEncoder;
        readonly SecuritySettings securitySettings;
        readonly ILogger logger;

        public ProductKeyService(
            IRepository<ProductKeyEntity> productKeyRepository,
            IHmacEncoder<GetProductKeyRequest> getRequestEncoder,
            IHmacEncoder<StoreProductKeyRequest> storeRequestEncoder,
            IHmacEncoder<UpdateProductKeyRequest> updateRequestEncoder,
            IHmacEncoder<ProductKeyResponse> productKeyResponseEncoder,
            SecuritySettings securitySettings,
            ILogger logger)
        {
            this.productKeyRepository = productKeyRepository;
            this.getRequestEncoder = getRequestEncoder;
            this.storeRequestEncoder = storeRequestEncoder;
            this.updateRequestEncoder = updateRequestEncoder;
            this.productKeyResponseEncoder = productKeyResponseEncoder;
            this.securitySettings = securitySettings;
            this.logger = logger;
        }

        public ProductKeyResponse GetProductKey(GetProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos = new List<LogInfo>
            {
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                new LogInfo(MyLogInfoKey.Key, request.Key),
                new LogInfo(MyLogInfoKey.Owner, request.Owner),
                new LogInfo(MyLogInfoKey.Status, request.Status),
                new LogInfo(MyLogInfoKey.Count, request.Count)
            };

            logger.Info(MyOperation.GetProductKey, OperationStatus.Started, logInfos);

            ValidateGetRequest(request);

            IEnumerable<ProductKey> productKeys = FindProductKeys(request, request.Count);

            if (productKeys?.Count() == 0)
            {
                Exception ex = new NullReferenceException("No key found for the given filters");
                logger.Info(MyOperation.GetProductKey, OperationStatus.Failure, ex, logInfos);

                throw ex;
            }

            ProductKeyResponse response = new ProductKeyResponse(productKeys.ToApiObjects());
            response.HmacToken = productKeyResponseEncoder.GenerateToken(response, securitySettings.SharedSecretKey);

            logger.Info(MyOperation.GetProductKey, OperationStatus.Success, logInfos);

            return response;
        }

        public void StoreProductKey(StoreProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos = new List<LogInfo>
            {
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                new LogInfo(MyLogInfoKey.Key, request.Key),
                new LogInfo(MyLogInfoKey.Owner, request.Owner),
                new LogInfo(MyLogInfoKey.Status, request.Status)
            };

            logger.Info(MyOperation.StoreProductKey, OperationStatus.Started, logInfos);

            ValidateStoreRequest(request);

            ProductKey productKey = CreateProductKeyFromRequest(request);
            StoreProductKey(productKey);

            logger.Debug(MyOperation.StoreProductKey, OperationStatus.Success, logInfos);
        }

        public void UpdateProductKey(UpdateProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos = new List<LogInfo>
            {
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                new LogInfo(MyLogInfoKey.Key, request.Key),
                new LogInfo(MyLogInfoKey.Owner, request.Owner),
                new LogInfo(MyLogInfoKey.Status, request.Status)
            };

            logger.Info(MyOperation.UpdateProductKey, OperationStatus.Started, logInfos);

            ValidateUpdateRequest(request);
            
            ProductKey productKey = CreateProductKeyFromRequest(request);
            UpdateProductKeyDetails(productKey);

            logger.Debug(MyOperation.UpdateProductKey, OperationStatus.Success, logInfos);
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
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                    new LogInfo(MyLogInfoKey.Key, request.Key));

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
            else if (DoesKeyExistInStore(request.Key))
            {
                exception = new ArgumentException("The specified product key already exists");
            }

            if (!(exception is null))
            {
                logger.Error(
                    MyOperation.StoreProductKey,
                    OperationStatus.Failure,
                    exception,
                    new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                    new LogInfo(MyLogInfoKey.Key, request.Key));

                throw exception;
            }
        }
        
        void ValidateUpdateRequest(UpdateProductKeyRequest request)
        {
            bool isTokenValid = updateRequestEncoder.IsTokenValid(request.HmacToken, request, securitySettings.SharedSecretKey);

            Exception exception = null;

            if (!isTokenValid && securitySettings.IsEnabled)
            {
                exception = new AuthenticationException("The provided HMAC token is not valid");
            }
            else if (string.IsNullOrWhiteSpace(request.Key))
            {
                exception = new ArgumentNullException("key");
            }
            else if (!DoesKeyExistInStore(request.Key))
            {
                exception = new ArgumentException("The specified product key does not exist");
            }

            if (!(exception is null))
            {
                logger.Error(
                    MyOperation.UpdateProductKey,
                    OperationStatus.Failure,
                    exception,
                    new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                    new LogInfo(MyLogInfoKey.Key, request.Key));

                throw exception;
            }
        }

        bool DoesKeyExistInStore(string key)
        {
            string id = GenerateKeyId(key);
            return productKeyRepository.TryGet(id) != null;
        }

        IEnumerable<ProductKey> FindProductKeys(GetProductKeyRequest request, int count)
        {
            IEnumerable<ProductKeyEntity> productKeyCandidates = productKeyRepository
                .GetAll()
                .Where(x =>
                    DoesPropertyMatchFilter(x.StoreName, request.StoreName) &&
                    DoesPropertyMatchFilter(x.ProductName, request.ProductName) &&
                    DoesPropertyMatchFilter(x.Key, request.Key) &&
                    DoesPropertyMatchFilter(x.Owner, request.Owner) &&
                    DoesPropertyMatchFilter(x.Status, request.Status));

            IList<ProductKeyEntity> shuffledCandidates = productKeyCandidates.Distinct().ToList().Shuffle();
            
            if (count > shuffledCandidates.Count)
            {
                count = shuffledCandidates.Count;
            }

            return shuffledCandidates.ToServiceModels().Take(count);
        }

        bool DoesPropertyMatchFilter(string value, string filterValue)
        {
            if (string.IsNullOrWhiteSpace(filterValue))
            {
                return true;
            }

            return value.Equals(filterValue, StringComparison.InvariantCultureIgnoreCase);
        }

        void StoreProductKey(ProductKey productKey)
        {
            productKeyRepository.Add(productKey.ToDataObject());
            productKeyRepository.ApplyChanges();
        }

        void UpdateProductKeyDetails(ProductKey productKey)
        {
            ProductKey productKeyToUpdate = productKeyRepository.Get(productKey.Id).ToServiceModel();

            if (!string.IsNullOrWhiteSpace(productKey.StoreName))
            {
                productKeyToUpdate.StoreName = productKey.StoreName;
            }
            
            if (!string.IsNullOrWhiteSpace(productKey.ProductName))
            {
                productKeyToUpdate.ProductName = productKey.ProductName;
            }
            
            if (!string.IsNullOrWhiteSpace(productKey.Owner))
            {
                productKeyToUpdate.Owner = productKey.Owner;
            }

            if (productKey.Status != ProductKeyStatus.Unknown)
            {
                productKeyToUpdate.Status = productKey.Status;
            }

            productKeyToUpdate.UpdatedDateTime = DateTime.Now;

            productKeyRepository.Update(productKeyToUpdate.ToDataObject());
            productKeyRepository.ApplyChanges();
        }

        string GenerateKeyId(string key)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(key));
                Guid result = new Guid(hash);

                return result.ToString();
            }
        }

        ProductKey CreateProductKeyFromRequest(StoreProductKeyRequest request)
        {
            ProductKey productKey = new ProductKey();
            productKey.Id = GenerateKeyId(request.Key);
            productKey.StoreName = request.StoreName;
            productKey.ProductName = request.ProductName;
            productKey.Key = request.Key;
            productKey.Owner = request.Owner;
            productKey.Status = ProductKeyStatus.FromName(request.Status);
            productKey.AddedDateTime = DateTime.Now;
            productKey.UpdatedDateTime = productKey.AddedDateTime;

            return productKey;
        }

        ProductKey CreateProductKeyFromRequest(UpdateProductKeyRequest request)
        {
            ProductKey productKey = new ProductKey();
            productKey.Id = GenerateKeyId(request.Key);
            productKey.StoreName = request.StoreName;
            productKey.ProductName = request.ProductName;
            productKey.Key = request.Key;
            productKey.Owner = request.Owner;
            productKey.Status = ProductKeyStatus.FromName(request.Status);

            return productKey;
        }
    }
}
