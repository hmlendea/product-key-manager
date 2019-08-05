using ProductKeyManager.Api.Models;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Service
{
    public interface IProductKeyService
    {
        void StoreProductKey(StoreProductKeyRequest request);

        ProductKey GetProductKey(GetProductKeyRequest request);
    }
}
