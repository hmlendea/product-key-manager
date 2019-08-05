using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Security
{
    public sealed class StoreProductKeyRequestHmacEncoder : HmacEncoder<StoreProductKeyRequest>
    {
        public override string GenerateToken(StoreProductKeyRequest obj, string sharedSecretKey)
        {
            string stringForSigning =
                obj.StoreName +
                obj.ProductName +
                obj.Key;

            string hmacToken = ComputeHmacToken(stringForSigning, sharedSecretKey);

            return hmacToken;
        }
    }
}
