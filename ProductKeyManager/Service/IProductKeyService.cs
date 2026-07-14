using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Service
{
    public interface IProductKeyService
    {
        void AddProductKey(AddProductKeyRequest request);

        ProductKeyResponse GetProductKey(GetProductKeyRequest request);

        void UpdateProductKey(UpdateProductKeyRequest request);
    }
}
