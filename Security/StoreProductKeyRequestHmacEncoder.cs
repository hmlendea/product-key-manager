using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Security
{
    public sealed class AddProductKeyRequestHmacEncoder : HmacEncoder<AddProductKeyRequest>
    {
        public override string GenerateToken(
            AddProductKeyRequest obj,
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
