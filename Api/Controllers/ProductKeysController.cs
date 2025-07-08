using Microsoft.AspNetCore.Mvc;
using NuciAPI.Controllers;
using ProductKeyManager.Api.Models;
using ProductKeyManager.Service;

namespace ProductKeyManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductKeysController(IProductKeyService service) : NuciApiController
    {
        [HttpGet]
        public ActionResult GetProductKey([FromBody] GetProductKeyRequest request)
            => ProcessRequest(request, () => service.GetProductKey(request));

        [HttpPost]
        public ActionResult AddProductKey([FromBody] AddProductKeyRequest request)
            => ProcessRequest(request, () => service.AddProductKey(request));

        [HttpPut]
        public ActionResult UpdateProductKey([FromBody] UpdateProductKeyRequest request)
            => ProcessRequest(request, () => service.UpdateProductKey(request));
    }
}
