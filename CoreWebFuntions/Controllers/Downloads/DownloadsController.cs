using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebFuntions.Controllers.Downloads.Actions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebFuntions.Controllers.Downloads
{
    [Route("api/[controller]")]
    public class DownloadsController : Controller
    {
        private readonly IMediator mediator;

        public DownloadsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]/{key}")]
        public async Task<ActionResult> Download(string key)
        {
            GetFileByKey.Response response = await this.mediator.Send(new GetFileByKey.Request() { Key = key });
            if (response == null)
            {
                return NotFound();
            }
            return File(response.Bytes, response.ContentType, response.FileName);
        }

        [HttpGet("list")]
        public async Task<ActionResult> GetList()
        {
            GetFiles.Response response = await this.mediator.Send(new GetFiles.Request());
            return new OkObjectResult(response.Rows);
        }
    }
}