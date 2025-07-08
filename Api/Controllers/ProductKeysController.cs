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
                return BadRequest(NuciApiErrorResponse.InvalidRequest);
            }

            try
            {
                return Ok(service.GetProductKey(request));
            }
            catch (SecurityException ex)
            {
                return Unauthorized(NuciApiErrorResponse.FromException(ex));
            }
            catch (Exception ex)
            {
                return BadRequest(NuciApiErrorResponse.FromException(ex));
            }
        }

        [HttpPost]
        public ActionResult AddProductKey([FromBody] AddProductKeyRequest request)
        {
            if (request is null)
            {
                return BadRequest(NuciApiErrorResponse.InvalidRequest);
            }

            try
            {
                service.AddProductKey(request);

                return Ok(NuciApiSuccessResponse.Default);
            }
            catch (SecurityException ex)
            {
                return Unauthorized(NuciApiErrorResponse.FromException(ex));
            }
            catch (Exception ex)
            {
                return BadRequest(NuciApiErrorResponse.FromException(ex));
            }
        }

        [HttpPut]
        public ActionResult UpdateProductKey([FromBody] UpdateProductKeyRequest request)
        {
            if (request is null)
            {
                return BadRequest(NuciApiErrorResponse.InvalidRequest);
            }

            try
            {
                service.UpdateProductKey(request);

                return Ok(NuciApiSuccessResponse.Default);
            }
            catch (SecurityException ex)
            {
                return Unauthorized(NuciApiErrorResponse.FromException(ex));
            }
            catch (Exception ex)
            {
                return BadRequest(NuciApiErrorResponse.FromException(ex));
            }
        }
    }
}
