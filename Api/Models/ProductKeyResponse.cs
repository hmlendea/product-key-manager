using System.Collections.Generic;
using System.Linq;
using NuciAPI.Responses;

namespace ProductKeyManager.Api.Models
{
    public sealed class ProductKeyResponse : SuccessResponse
    {
        public IEnumerable<ProductKeyObject> ProductKeys { get; set; }

        public int Count => ProductKeys.Count();

        public ProductKeyResponse(ProductKeyObject productKey)
            => ProductKeys = [productKey];

        public ProductKeyResponse(IEnumerable<ProductKeyObject> productKeys)
            => ProductKeys = productKeys;
    }
}
