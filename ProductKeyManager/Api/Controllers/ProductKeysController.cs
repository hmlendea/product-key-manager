using Microsoft.AspNetCore.Mvc;
using NuciAPI.Controllers;
using ProductKeyManager.Api.Models;
using ProductKeyManager.Configuration;
using ProductKeyManager.Service;

namespace ProductKeyManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductKeysController(
        IProductKeyService service,
        SecuritySettings securitySettings) : NuciApiController
    {
        readonly NuciApiAuthorisation authorisation = NuciApiAuthorisation.ApiKey(securitySettings.SharedSecretKey);

        [HttpGet]
        public ActionResult GetProductKey([FromBody] GetProductKeyRequest request)
            => ProcessRequest(request, () => service.GetProductKey(request), authorisation);

        [HttpPost]
        public ActionResult AddProductKey([FromBody] AddProductKeyRequest request)
            => ProcessRequest(request, () => service.AddProductKey(request), authorisation);

        [HttpPut]
        public ActionResult UpdateProductKey([FromBody] UpdateProductKeyRequest request)
            => ProcessRequest(request, () => service.UpdateProductKey(request), authorisation);
    }
}
