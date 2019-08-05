using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Service
{
    public interface IProductKeyService
    {
        void StoreProductKey(StoreProductKeyRequest request);

        ProductKeyResponse GetProductKey(GetProductKeyRequest request);

        void UpdateProductKey(UpdateProductKeyRequest request);
    }
}
