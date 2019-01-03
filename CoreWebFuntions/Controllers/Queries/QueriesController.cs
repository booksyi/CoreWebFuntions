using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebFuntions.Controllers.Queries.Actions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebFuntions.Controllers.Queries
{
    [Route("api/[controller]")]
    public class QueriesController : Controller
    {
        private readonly IMediator mediator;

        public QueriesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Query([FromQuery] GetQuery.Request request)
        {
            GetQuery.Response response = await this.mediator.Send(request);
            return new OkObjectResult(response.Rows);
        }

        [HttpGet("insertScript/{tableName}")]
        public async Task<ActionResult> GetInsertScript(string tableName, [FromQuery] GetInsertScript.Request request)
        {
            request.TableName = tableName;
            GetInsertScript.Response response = await this.mediator.Send(request);
            return Content(string.Join("\r\n", response.Lines));
        }
    }
}