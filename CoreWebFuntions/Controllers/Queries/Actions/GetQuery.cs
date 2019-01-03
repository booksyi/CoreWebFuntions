using CoreWebFuntions.Data;
using HelpersForCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Queries.Actions
{
    /// <summary>
    /// 將 Select 的結果丟回來
    /// </summary>
    public class GetQuery
    {
        public class Request : IRequest<Response>
        {
            public string CommandText { get; set; }
        }

        public class Response
        {
            public IEnumerable<Dictionary<string, object>> Rows { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DatabaseContext context;

            public Handler(DatabaseContext context)
            {
                this.context = context;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                Response response = new Response();
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    await context.Database.OpenConnectionAsync();
                    command.CommandText = request.CommandText;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        response.Rows = dt.Rows.Cast<DataRow>().Take(1000).Select(x => x.ToDictionary());
                    }
                    context.Database.CloseConnection();
                }
                return response;
            }
        }
    }
}
