using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Service;

namespace ProductKeyManager.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        readonly IProductKeyService service;

        public RewardsController(IProductKeyService service)
        {
            this.service = service;
        }
        
        [HttpGet]
        public ActionResult GetAccount()
        {
            return Ok();
        }

        [HttpPost]
        public ActionResult RecordReward(
            [FromQuery] string username,
            [FromQuery] string gaProvider,
            [FromQuery] string gaId,
            [FromQuery] string steamUsername,
            [FromQuery] string steamAppId,
            [FromQuery] string activationKey,
            [FromQuery] string hmac)
        {
            try
            {
                GetProductKeyRequest request = new GetProductKeyRequest
                {
                    HmacToken = hmac
                };

                service.GetProductKey(request);

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
