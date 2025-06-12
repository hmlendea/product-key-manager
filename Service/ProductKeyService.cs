using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
    public class ProductKeyService(
        IRepository<ProductKeyEntity> productKeyRepository,
        IHmacEncoder<GetProductKeyRequest> getRequestEncoder,
        IHmacEncoder<AddProductKeyRequest> storeRequestEncoder,
        IHmacEncoder<UpdateProductKeyRequest> updateRequestEncoder,
        IHmacEncoder<ProductKeyResponse> productKeyResponseEncoder,
        SecuritySettings securitySettings,
        ILogger logger) : IProductKeyService
    {
        readonly IRepository<ProductKeyEntity> productKeyRepository = productKeyRepository;
        readonly IHmacEncoder<GetProductKeyRequest> getRequestEncoder = getRequestEncoder;
        readonly IHmacEncoder<AddProductKeyRequest> storeRequestEncoder = storeRequestEncoder;
        readonly IHmacEncoder<UpdateProductKeyRequest> updateRequestEncoder = updateRequestEncoder;
        readonly IHmacEncoder<ProductKeyResponse> productKeyResponseEncoder = productKeyResponseEncoder;
        readonly SecuritySettings securitySettings = securitySettings;
        readonly ILogger logger = logger;

        public ProductKeyResponse GetProductKey(GetProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos =
            [
                new(MyLogInfoKey.StoreName, request.StoreName),
                new(MyLogInfoKey.ProductName, request.ProductName),
                new(MyLogInfoKey.Key, request.Key),
                new(MyLogInfoKey.Owner, request.Owner),
                new(MyLogInfoKey.Status, request.Status),
                new(MyLogInfoKey.Count, request.Count)
            ];

            logger.Info(MyOperation.GetProductKey, OperationStatus.Started, logInfos);

            ValidateGetRequest(request);

            IEnumerable<ProductKey> productKeys = FindProductKeys(request, request.Count)
                .OrderBy(x => x.ProductName)
                .ThenBy(x => x.Key);

            if (productKeys?.Count() == 0)
            {
                Exception ex = new NullReferenceException("No key found for the given filters");
                logger.Info(MyOperation.GetProductKey, OperationStatus.Failure, ex, logInfos);

                throw ex;
            }

            ProductKeyResponse response = new(productKeys.ToApiObjects());
            response.HmacToken = productKeyResponseEncoder.GenerateToken(response, securitySettings.SharedSecretKey);

            logger.Info(MyOperation.GetProductKey, OperationStatus.Success, logInfos);

            return response;
        }

        public void AddProductKey(AddProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos =
            [
                new(MyLogInfoKey.StoreName, request.StoreName),
                new(MyLogInfoKey.ProductName, request.ProductName),
                new(MyLogInfoKey.Key, request.Key),
                new(MyLogInfoKey.Owner, request.Owner),
                new(MyLogInfoKey.Status, request.Status),
                new(MyLogInfoKey.Comment, request.Comment)
            ];

            logger.Info(MyOperation.AddProductKey, OperationStatus.Started, logInfos);

            ValidateStoreRequest(request);

            ProductKey productKey = CreateProductKeyFromRequest(request);
            AddProductKey(productKey);

            logger.Debug(MyOperation.AddProductKey, OperationStatus.Success, logInfos);
        }

        public void UpdateProductKey(UpdateProductKeyRequest request)
        {
            IEnumerable<LogInfo> logInfos =
            [
                new(MyLogInfoKey.StoreName, request.StoreName),
                new(MyLogInfoKey.ProductName, request.ProductName),
                new(MyLogInfoKey.Key, request.Key),
                new(MyLogInfoKey.Owner, request.Owner),
                new(MyLogInfoKey.Status, request.Status),
                new(MyLogInfoKey.Comment, request.Comment)
            ];

            logger.Info(MyOperation.UpdateProductKey, OperationStatus.Started, logInfos);

            ValidateUpdateRequest(request);

            ProductKey productKey = CreateProductKeyFromRequest(request);
            UpdateProductKeyDetails(productKey);

            logger.Debug(MyOperation.UpdateProductKey, OperationStatus.Success, logInfos);
        }

        void ValidateGetRequest(GetProductKeyRequest request)
        {
            if (getRequestEncoder.IsTokenValid(request.HmacToken, request, securitySettings.SharedSecretKey))
            {
                return;
            }

            AuthenticationException exception = new("The provided HMAC token is not valid");

            logger.Error(
                MyOperation.GetProductKey,
                OperationStatus.Failure,
                exception,
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                new LogInfo(MyLogInfoKey.Key, request.Key));

            throw exception;
        }

        void ValidateStoreRequest(AddProductKeyRequest request)
        {
            Exception exception;

            if (!storeRequestEncoder.IsTokenValid(request.HmacToken, request, securitySettings.SharedSecretKey))
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
            else
            {
                return;
            }

            logger.Error(
                MyOperation.AddProductKey,
                OperationStatus.Failure,
                exception,
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                new LogInfo(MyLogInfoKey.Key, request.Key));

            throw exception;
        }

        void ValidateUpdateRequest(UpdateProductKeyRequest request)
        {
            Exception exception;

            if (!updateRequestEncoder.IsTokenValid(request.HmacToken, request, securitySettings.SharedSecretKey))
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
            else
            {
                return;
            }

            logger.Error(
                MyOperation.UpdateProductKey,
                OperationStatus.Failure,
                exception,
                new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                new LogInfo(MyLogInfoKey.Key, request.Key));

            throw exception;
        }

        bool DoesKeyExistInStore(string key)
            => productKeyRepository.TryGet(GenerateKeyId(key)) is not null;

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

        static bool DoesPropertyMatchFilter(string value, string filterValue)
        {
            if (string.IsNullOrWhiteSpace(filterValue))
            {
                return true;
            }

            if (value is null)
            {
                return false;
            }

            string pattern = filterValue;

            if (pattern[0] != '^' && pattern[pattern.Length - 1] != '$')
            {
                pattern = $"^{pattern}$";
            }

            return Regex.IsMatch(value, pattern);
        }

        void AddProductKey(ProductKey productKey)
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

            if (!string.IsNullOrWhiteSpace(productKey.Comment))
            {
                productKeyToUpdate.Comment = productKey.Comment;
            }

            if (productKey.Status != ProductKeyStatus.Unknown)
            {
                productKeyToUpdate.Status = productKey.Status;
            }

            productKeyToUpdate.UpdatedDateTime = DateTime.Now;

            productKeyRepository.Update(productKeyToUpdate.ToDataObject());
            productKeyRepository.ApplyChanges();
        }

        static string GenerateKeyId(string key)
            => new Guid(MD5.HashData(Encoding.Default.GetBytes(key))).ToString();

        static ProductKey CreateProductKeyFromRequest(AddProductKeyRequest request)
        {
            ProductKey productKey = new()
            {
                Id = GenerateKeyId(request.Key),
                StoreName = request.StoreName,
                ProductName = request.ProductName,
                Key = request.Key,
                Owner = request.Owner,
                Comment = request.Comment,
                Status = ProductKeyStatus.FromName(request.Status),
                AddedDateTime = DateTime.Now
            };
            productKey.UpdatedDateTime = productKey.AddedDateTime;

            return productKey;
        }

        static ProductKey CreateProductKeyFromRequest(UpdateProductKeyRequest request) => new()
        {
            Id = GenerateKeyId(request.Key),
            StoreName = request.StoreName,
            ProductName = request.ProductName,
            Key = request.Key,
            Owner = request.Owner,
            Comment = request.Comment,
            Status = ProductKeyStatus.FromName(request.Status)
        };
    }
}
