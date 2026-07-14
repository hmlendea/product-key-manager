using ProductKeyManager.Api.Models;

namespace ProductKeyManager.Service
{
    public interface IProductKeyService
    {
        void AddProductKey(AddProductKeyRequest request);

        GetProductKeyResponse GetProductKey(GetProductKeyRequest request);

        void UpdateProductKey(UpdateProductKeyRequest request);
    }
}
