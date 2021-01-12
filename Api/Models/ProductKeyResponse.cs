using System.Collections.Generic;
using System.Linq;

namespace ProductKeyManager.Api.Models
{
    public sealed class ProductKeyResponse : SuccessResponse
    {
        public IEnumerable<ProductKeyObject> ProductKeys { get; set; }

        public int Count => ProductKeys.Count();

        public ProductKeyResponse(ProductKeyObject productKey)
        {
            ProductKeys = new List<ProductKeyObject> { productKey };
        }

        public ProductKeyResponse(IEnumerable<ProductKeyObject> productKeys)
        {
            ProductKeys = productKeys;
        }
    }
}
