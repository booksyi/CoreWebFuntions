using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebFuntions.Controllers.Combines.Actions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebFuntions.Controllers.Combines
{
    [Route("api/[controller]")]
    public class CombinesController : Controller
    {
        private readonly IMediator mediator;

        public CombinesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // /api/Combines/CombineJson
        [HttpGet("[action]")]
        public async Task<ActionResult> CombineJson([FromQuery] CombineJson.Request request)
        {
            var response = await this.mediator.Send(request);
            return new OkObjectResult(response);
        }
    }
}