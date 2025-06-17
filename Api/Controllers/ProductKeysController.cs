using System;
using System.Security;
using Microsoft.AspNetCore.Mvc;
using NuciAPI.Responses;
using ProductKeyManager.Api.Models;
using ProductKeyManager.Service;

namespace ProductKeyManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductKeysController(IProductKeyService service) : ControllerBase
    {
        [HttpGet]
        public ActionResult GetProductKey([FromBody] GetProductKeyRequest request)
        {
            if (request is null)
            {
                return BadRequest(ErrorResponse.InvalidRequest);
            }

            try
            {
                return Ok(service.GetProductKey(request));
            }
            catch (SecurityException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.FromException(ex));
            }
        }

        [HttpPost]
        public ActionResult AddProductKey([FromBody] AddProductKeyRequest request)
        {
            if (request is null)
            {
                return BadRequest(ErrorResponse.InvalidRequest);
            }

            try
            {
                service.AddProductKey(request);

                return Ok(SuccessResponse.Default);
            }
            catch (SecurityException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.FromException(ex));
            }
        }

        [HttpPut]
        public ActionResult UpdateProductKey([FromBody] UpdateProductKeyRequest request)
        {
            if (request is null)
            {
                return BadRequest(ErrorResponse.InvalidRequest);
            }

            try
            {
                service.UpdateProductKey(request);

                return Ok(SuccessResponse.Default);
            }
            catch (SecurityException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.FromException(ex));
            }
        }
    }
}
