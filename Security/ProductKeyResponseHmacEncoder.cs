using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Security
{
    public sealed class ProductKeyResponseHmacEncoder : HmacEncoder<ProductKeyResponse>
    {
        public override string GenerateToken(ProductKeyResponse obj, string sharedSecretKey)
        {
            string stringForSigning = string.Empty;

            foreach (ProductKeyObject productKey in obj.ProductKeys)
            {
                if (productKey is null)
                {
                    continue;
                }

                stringForSigning +=
                    productKey.Store +
                    productKey.Product +
                    productKey.Key +
                    productKey.Owner +
                    productKey.Comment +
                    productKey.Status;
            }

            return ComputeHmacToken(stringForSigning, sharedSecretKey);
        }
    }
}
