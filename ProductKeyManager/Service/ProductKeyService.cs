using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using NuciDAL.Repositories;

using NuciExtensions;

using NuciLog.Core;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Configuration;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Logging;
using ProductKeyManager.Service.Mapping;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Service
{
    public sealed class ProductKeyService(
        IFileRepository<ProductKeyDataObject> productKeyRepository,
        SecuritySettings securitySettings,
        ILogger logger) : IProductKeyService
    {
        public GetProductKeyResponse GetProductKey(GetProductKeyRequest request)
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

            IEnumerable<ProductKey> productKeys = FindProductKeys(request, request.Count)
                .OrderBy(productKey => productKey.ProductName)
                .ThenBy(productKey => productKey.Key);

            if (EnumerableExt.IsNullOrEmpty(productKeys))
            {
                Exception exception = new NullReferenceException("No key found for the given filters");
                logger.Info(MyOperation.GetProductKey, OperationStatus.Failure, exception, logInfos);

                throw exception;
            }

            GetProductKeyResponse response = new(productKeys.ToApiObjects());
            response.SignHMAC(securitySettings.SharedSecretKey);

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

            ProductKey productKey = CreateProductKeyFromRequest(request);
            UpdateProductKeyDetails(productKey);

            logger.Debug(MyOperation.UpdateProductKey, OperationStatus.Success, logInfos);
        }

        private IEnumerable<ProductKey> FindProductKeys(GetProductKeyRequest request, int count)
        {
            IEnumerable<ProductKeyDataObject> shuffledCandidates = productKeyRepository
                .GetAll()
                .Where(dataObject =>
                    DoesPropertyMatchFilter(dataObject.StoreName, request.StoreName) &&
                    DoesPropertyMatchFilter(dataObject.ProductName, request.ProductName) &&
                    DoesPropertyMatchFilter(dataObject.Key, request.Key) &&
                    DoesPropertyMatchFilter(dataObject.Owner, request.Owner) &&
                    DoesPropertyMatchFilter(dataObject.Status, request.Status))
                .Distinct()
                .ToList()
                .Shuffle();

            return shuffledCandidates
                .ToDomainModels()
                .Take(Math.Min(count, shuffledCandidates.Count()));
        }

        private static bool DoesPropertyMatchFilter(string value, string filterValue)
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

            if (pattern[0] != '^' && pattern[^1] != '$')
            {
                pattern = $"^{pattern}$";
            }

            return Regex.IsMatch(value, pattern);
        }

        private void AddProductKey(ProductKey productKey)
        {
            productKeyRepository.Add(productKey.ToDataObject());
            productKeyRepository.SaveChanges();
        }

        private void UpdateProductKeyDetails(ProductKey productKey)
        {
            ProductKey productKeyToUpdate = productKeyRepository.Get(productKey.Id).ToDomainModel();

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
            productKeyRepository.SaveChanges();
        }

        private static string GenerateKeyId(string key)
            => new Guid(MD5.HashData(Encoding.Default.GetBytes(key))).ToString();

        private static ProductKey CreateProductKeyFromRequest(AddProductKeyRequest request)
        {
            DateTime addedDateTime = DateTime.Now;

            return new()
            {
                Id = GenerateKeyId(request.Key),
                StoreName = request.StoreName,
                ProductName = request.ProductName,
                Key = request.Key,
                Owner = request.Owner,
                Comment = request.Comment,
                Status = ProductKeyStatus.FromName(request.Status),
                AddedDateTime = addedDateTime,
                UpdatedDateTime = addedDateTime
            };
        }

        private static ProductKey CreateProductKeyFromRequest(UpdateProductKeyRequest request) => new()
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
