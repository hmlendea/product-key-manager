using System;

using Microsoft.AspNetCore.Mvc;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Service;

namespace ProductKeyManager.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductKeysController(IProductKeyService service) : ControllerBase
    {
        readonly IProductKeyService service = service;

        [HttpGet]
        public ActionResult GetProductKey([FromQuery] ProductKeyQuery query)
        {
            try
            {
                GetProductKeyRequest request = new()
                {
                    StoreName = query.Store,
                    ProductName = query.Product,
                    Key = query.Key,
                    Owner = query.Owner,
                    Status = query.Status,
                    Count = query.Count ?? 1,
                    HmacToken = query.Hmac
                };

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
        public ActionResult StoreProductKey([FromQuery] ProductKeyQuery query)
        {
            try
            {
                StoreProductKeyRequest request = new()
                {
                    StoreName = query.Store,
                    ProductName = query.Product,
                    Key = query.Key,
                    Owner = query.Owner,
                    Comment = query.Comment,
                    Status = query.Status,
                    HmacToken = query.Hmac
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
        public ActionResult UpdateProductKey([FromQuery] ProductKeyQuery query)
        {
            try
            {
                UpdateProductKeyRequest request = new()
                {
                    StoreName = query.Store,
                    ProductName = query.Product,
                    Key = query.Key,
                    Owner = query.Owner,
                    Comment = query.Comment,
                    Status = query.Status,
                    HmacToken = query.Hmac
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
