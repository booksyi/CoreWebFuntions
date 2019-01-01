using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebFuntions.Controllers.Functions.Actions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebFuntions.Controllers.Functions
{
    [Route("api/[controller]")]
    public class FunctionsController : Controller
    {
        private readonly IMediator mediator;

        public FunctionsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Crawler.Response>> Crawler([FromQuery] Crawler.Request request)
        {
            Crawler.Response response = await this.mediator.Send(request);
            return new OkObjectResult(response);
        }
    }
}