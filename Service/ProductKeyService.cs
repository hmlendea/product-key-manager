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
        readonly SecuritySettings securitySettings;
        readonly ILogger logger;

        public ProductKeyService(
            IRepository<ProductKeyEntity> productKeyRepository,
            IHmacEncoder<GetProductKeyRequest> getRequestEncoder,
            SecuritySettings securitySettings,
            ILogger logger)
        {
            this.productKeyRepository = productKeyRepository;
            this.getRequestEncoder = getRequestEncoder;
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

            ProductKey productKey = GetProductKeyFromRequest(request);

            if (productKey is null)
            {
                logger.Info(MyOperation.GetProductKey, OperationStatus.Failure, logInfos);
                return null;
            }
            
            logger.Info(MyOperation.GetProductKey, OperationStatus.Success, logInfos);
            return productKey;
        }
        
        void ValidateGetRequest(GetProductKeyRequest request)
        {
            bool isTokenValid = getRequestEncoder.IsTokenValid(request.HmacToken, request, securitySettings.SharedSecretKey);

            if (!isTokenValid)
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

        ProductKey GetProductKeyFromRequest(GetProductKeyRequest request)
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
                    x => x.StoreName.Equals( request.ProductName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (productKeyCandidates.Any())
            {
                return productKeyCandidates.GetRandomElement().ToServiceModel();
            }

            return null;
        }

        bool WasRewardAlreadyRecorded(ProductKey reward)
        {
            return productKeyRepository.TryGet(reward.Id) != null;
        }

        void StoreProductKey(ProductKey productKey)
        {
            productKeyRepository.Add(productKey.ToDataObject());
            productKeyRepository.ApplyChanges();
        }
    }
}
