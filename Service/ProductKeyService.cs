using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
        IFileRepository<ProductKeyEntity> productKeyRepository,
        SecuritySettings securitySettings,
        ILogger logger) : IProductKeyService
    {
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

            if (EnumerableExt.IsNullOrEmpty(productKeys))
            {
                Exception ex = new NullReferenceException("No key found for the given filters");
                logger.Info(MyOperation.GetProductKey, OperationStatus.Failure, ex, logInfos);

                throw ex;
            }

            ProductKeyResponse response = new(productKeys.ToApiObjects());
            response.HmacToken = HmacEncoder.GenerateToken(response, securitySettings.SharedSecretKey);

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
            try
            {
                HmacValidator.Validate(request.HmacToken, request, securitySettings.SharedSecretKey);
            }
            catch (SecurityException ex)
            {
                logger.Error(
                    MyOperation.GetProductKey,
                    OperationStatus.Failure,
                    ex,
                    new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                    new LogInfo(MyLogInfoKey.Key, request.Key));

                throw;
            }
        }

        void ValidateStoreRequest(AddProductKeyRequest request)
        {
            try
            {
                HmacValidator.Validate(request.HmacToken, request, securitySettings.SharedSecretKey);

                ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Key);

                if (DoesKeyExistInStore(request.Key))
                {
                    throw new ArgumentException("The specified product key already exists");
                }
            }
            catch (SecurityException ex)
            {
                logger.Error(
                    MyOperation.AddProductKey,
                    OperationStatus.Failure,
                    ex,
                    new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                    new LogInfo(MyLogInfoKey.Key, request.Key));

                throw;
            }
        }

        void ValidateUpdateRequest(UpdateProductKeyRequest request)
        {
            try
            {
                HmacValidator.Validate(request.HmacToken, request, securitySettings.SharedSecretKey);

                ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Key);

                if (DoesKeyExistInStore(request.Key))
                {
                    throw new ArgumentException("The specified product key already exists");
                }
            }
            catch (SecurityException ex)
            {
                logger.Error(
                    MyOperation.UpdateProductKey,
                    OperationStatus.Failure,
                    ex,
                    new LogInfo(MyLogInfoKey.StoreName, request.StoreName),
                    new LogInfo(MyLogInfoKey.ProductName, request.ProductName),
                    new LogInfo(MyLogInfoKey.Key, request.Key));

                throw;
            }
        }

        bool DoesKeyExistInStore(string key)
            => productKeyRepository.TryGet(GenerateKeyId(key)) is not null;

        IEnumerable<ProductKey> FindProductKeys(GetProductKeyRequest request, int count)
        {
            IList<ProductKeyEntity> shuffledCandidates = productKeyRepository
                .GetAll()
                .Where(x =>
                    DoesPropertyMatchFilter(x.StoreName, request.StoreName) &&
                    DoesPropertyMatchFilter(x.ProductName, request.ProductName) &&
                    DoesPropertyMatchFilter(x.Key, request.Key) &&
                    DoesPropertyMatchFilter(x.Owner, request.Owner) &&
                    DoesPropertyMatchFilter(x.Status, request.Status))
                .Distinct()
                .ToList()
                .Shuffle();

            return shuffledCandidates
                .ToServiceModels()
                .Take(Math.Min(count, shuffledCandidates.Count));
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

            if (pattern[0].NotEquals('^') && pattern[^1].NotEquals('$'))
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

            if (productKey.Status.NotEquals(ProductKeyStatus.Unknown))
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
