using System;

using Microsoft.AspNetCore.Mvc;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Service;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductKeysController : ControllerBase
    {
        readonly IProductKeyService service;

        public ProductKeysController(IProductKeyService service)
        {
            this.service = service;
        }

        [HttpGet]
        public ActionResult GetRandom(
            [FromQuery] string storeName,
            [FromQuery] string productName,
            [FromQuery] string hmac)
        {
            try
            {
                GetProductKeyRequest request = new GetProductKeyRequest
                {
                    StoreName = storeName,
                    ProductName = productName,
                    HmacToken = hmac
                };

                ProductKey key = service.GetProductKey(request);

                return Ok(key);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return BadRequest(response);
            }
        }
    }
}
