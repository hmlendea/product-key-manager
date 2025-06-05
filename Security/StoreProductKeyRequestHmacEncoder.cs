using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Security
{
    public sealed class StoreProductKeyRequestHmacEncoder : HmacEncoder<StoreProductKeyRequest>
    {
        public override string GenerateToken(
            StoreProductKeyRequest obj,
            string sharedSecretKey)
            => ComputeHmacToken(
                obj.StoreName +
                obj.ProductName +
                obj.Key +
                obj.Owner +
                obj.Comment +
                obj.Status,
                sharedSecretKey);
    }
}
