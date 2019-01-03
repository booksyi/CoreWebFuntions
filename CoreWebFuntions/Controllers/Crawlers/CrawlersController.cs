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

        [HttpGet("[action]")]
        public async Task<ActionResult<GetEbrd.Response>> Crawler([FromQuery] GetEbrd.Request request)
        {
            GetEbrd.Response response = await this.mediator.Send(request);
            return new OkObjectResult(response);
        }
    }
}