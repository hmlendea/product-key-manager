using ProductKeyManager.Api.Models;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Service
{
    public interface IProductKeyService
    {
        ProductKey GetProductKey(GetProductKeyRequest request);
    }
}
