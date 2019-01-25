using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebFuntions.Controllers.Crawlers.Actions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebFuntions.Controllers.Crawlers
{
    [Route("api/[controller]")]
    public class CrawlersController : Controller
    {
        private readonly IMediator mediator;

        public CrawlersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // /api/crawlers/crawler?PageFrom=
        [HttpGet("[action]")]
        public async Task<ActionResult> Crawler([FromQuery] GetAdb.Request request)
        {
            var response = await this.mediator.Send(request);
            return new OkObjectResult(response);
        }

        // /api/crawlers/getAdb2
        [HttpGet("getAdb2")]
        public async Task<ActionResult> Crawler([FromQuery] GetAdb2.Request request)
        {
            var response = await this.mediator.Send(request);
            return new OkObjectResult(response);
        }
    }
}