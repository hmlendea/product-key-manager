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
        public ActionResult GetProductKey(
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

        [HttpPost]
        public ActionResult AddProductKey(
            [FromQuery] string storeName,
            [FromQuery] string productName,
            [FromQuery] string key,
            [FromQuery] string hmac)
        {
            try
            {
                StoreProductKeyRequest request = new StoreProductKeyRequest
                {
                    StoreName = storeName,
                    ProductName = productName,
                    Key = key,
                    HmacToken = hmac
                };

                service.StoreProductKey(request);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return BadRequest(response);
            }
        }

        [HttpPut]
        public ActionResult UpdateProductKey(
            [FromQuery] string storeName,
            [FromQuery] string productName,
            [FromQuery] string key,
            [FromQuery] string hmac)
        {
            try
            {
                UpdateProductKeyRequest request = new UpdateProductKeyRequest
                {
                    StoreName = storeName,
                    ProductName = productName,
                    Key = key,
                    HmacToken = hmac
                };

                service.UpdateProductKey(request);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return BadRequest(response);
            }
        }
    }
}
