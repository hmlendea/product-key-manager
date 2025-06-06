using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Security
{
    public sealed class UpdateProductKeyRequestHmacEncoder : HmacEncoder<UpdateProductKeyRequest>
    {
        public override string GenerateToken(
            UpdateProductKeyRequest obj,
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
