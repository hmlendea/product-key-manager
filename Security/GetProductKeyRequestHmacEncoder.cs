using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Security
{
    public sealed class GetProductKeyRequestHmacEncoder : HmacEncoder<GetProductKeyRequest>
    {
        public override string GenerateToken(
            GetProductKeyRequest obj,
            string sharedSecretKey)
            => ComputeHmacToken(
                obj.StoreName +
                obj.ProductName +
                obj.Owner,
                sharedSecretKey);
    }
}
