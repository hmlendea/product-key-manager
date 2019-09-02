using System;

using Microsoft.AspNetCore.Mvc;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Service;

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
            [FromQuery] string store,
            [FromQuery] string product,
            [FromQuery] string status,
            [FromQuery] string hmac)
        {
            try
            {
                GetProductKeyRequest request = new GetProductKeyRequest
                {
                    StoreName = store,
                    ProductName = product,
                    Status = status,
                    HmacToken = hmac
                };

                ProductKeyResponse response = service.GetProductKey(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public ActionResult StoreProductKey(
            [FromQuery] string store,
            [FromQuery] string product,
            [FromQuery] string key,
            [FromQuery] string status,
            [FromQuery] string hmac)
        {
            try
            {
                StoreProductKeyRequest request = new StoreProductKeyRequest
                {
                    StoreName = store,
                    ProductName = product,
                    Key = key,
                    Status = status,
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
            [FromQuery] string store,
            [FromQuery] string product,
            [FromQuery] string key,
            [FromQuery] string status,
            [FromQuery] string hmac)
        {
            try
            {
                UpdateProductKeyRequest request = new UpdateProductKeyRequest
                {
                    StoreName = store,
                    ProductName = product,
                    Key = key,
                    Status = status,
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
