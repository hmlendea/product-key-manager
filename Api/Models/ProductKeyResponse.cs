using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using NuciAPI.Responses;
using NuciSecurity.HMAC;

namespace ProductKeyManager.Api.Models
{
    public sealed class ProductKeyResponse : NuciApiSuccessResponse
    {
        [JsonPropertyName("products")]
        public IEnumerable<ProductKeyObject> ProductKeys { get; set; }

        [HmacIgnore]
        [JsonPropertyName("count")]
        public int Count => ProductKeys.Count();

        public ProductKeyResponse(ProductKeyObject productKey)
            => ProductKeys = [productKey];

        public ProductKeyResponse(IEnumerable<ProductKeyObject> productKeys)
            => ProductKeys = productKeys;
    }
}
