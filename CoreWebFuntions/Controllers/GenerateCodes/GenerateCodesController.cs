using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebFuntions.Controllers.GenerateCodes.Actions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebFuntions.Controllers.GenerateCodes
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateCodesController : ControllerBase
    {
        private readonly IMediator mediator;

        public GenerateCodesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Sample([FromQuery] Sample.Request request)
        {
            var response = await this.mediator.Send(request);
            return Content(response.Code);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Test1([FromQuery] Test1.Request request)
        {
            var response = await this.mediator.Send(request);
            return Content(response.Code);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Test2([FromQuery] Test2.Request request)
        {
            var response = await this.mediator.Send(request);
            return Ok(response.Code);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Test3([FromQuery] Test3.Request request)
        {
            var response = await this.mediator.Send(request);
            return Ok(response.Code);
        }
    }
}