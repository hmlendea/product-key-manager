using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Security
{
    public sealed class GetProductKeyRequestHmacEncoder : HmacEncoder<GetProductKeyRequest>
    {
        public override string GenerateToken(GetProductKeyRequest obj, string sharedSecretKey)
        {
            string stringForSigning =
                obj.StoreName +
                obj.ProductName +
                obj.Owner;

            string hmacToken = ComputeHmacToken(stringForSigning, sharedSecretKey);

            return hmacToken;
        }
    }
}
