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
            [FromQuery] string key,
            [FromQuery] string owner,
            [FromQuery] string status,
            [FromQuery] string count,
            [FromQuery] string hmac)
        {
            try
            {
                GetProductKeyRequest request = new()
                {
                    StoreName = store,
                    ProductName = product,
                    Key = key,
                    Owner = owner,
                    Status = status,
                    HmacToken = hmac
                };

                if (string.IsNullOrWhiteSpace(count))
                {
                    request.Count = 1;
                }
                else
                {
                    int.TryParse(count, out int parsedCount);
                    request.Count = Math.Max(1, parsedCount);
                }

                ProductKeyResponse response = service.GetProductKey(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new(ex);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public ActionResult StoreProductKey(
            [FromQuery] string store,
            [FromQuery] string product,
            [FromQuery] string key,
            [FromQuery] string owner,
            [FromQuery] string comment,
            [FromQuery] string status,
            [FromQuery] string hmac)
        {
            try
            {
                StoreProductKeyRequest request = new()
                {
                    StoreName = store,
                    ProductName = product,
                    Key = key,
                    Owner = owner,
                    Comment = comment,
                    Status = status,
                    HmacToken = hmac
                };

                service.StoreProductKey(request);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorResponse response = new(ex);
                return BadRequest(response);
            }
        }

        [HttpPut]
        public ActionResult UpdateProductKey(
            [FromQuery] string store,
            [FromQuery] string product,
            [FromQuery] string key,
            [FromQuery] string owner,
            [FromQuery] string comment,
            [FromQuery] string status,
            [FromQuery] string hmac)
        {
            try
            {
                UpdateProductKeyRequest request = new()
                {
                    StoreName = store,
                    ProductName = product,
                    Key = key,
                    Owner = owner,
                    Comment = comment,
                    Status = status,
                    HmacToken = hmac
                };

                service.UpdateProductKey(request);

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorResponse response = new(ex);
                return BadRequest(response);
            }
        }
    }
}
