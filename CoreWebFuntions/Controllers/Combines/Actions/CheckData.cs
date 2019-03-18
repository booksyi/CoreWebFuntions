using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Combines.Actions
{
    public class CheckData
    {
        public class Request : IRequest<Response>
        {
        }

        public class Row
        {
            public int id { get; set; }
            public string project_country { get; set; }
            public string sector { get; set; }
            public string project_date { get; set; }
            public string executing_agency { get; set; }
            public string contractor_name { get; set; }
            public string contractor_address { get; set; }
            public decimal total_contract_amount { get; set; }
            public decimal financed_by_adb { get; set; }
        }

        public class Response
        {
            public List<string> Result { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Handler()
            {
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                var rows = JsonConvert.DeserializeObject<Row[]>(await File.ReadAllTextAsync(@"D:\temp\output\page-from-1-to-177.txt"));
                int[] ids = rows.Where(x => x.project_country == x.sector).Select(x => x.id).ToArray();
                return new Response() { Result = ids.Select(x => $"page: {Convert.ToInt32(x / 20) + 1}, row: {x - (Convert.ToInt32(x / 20) * 20) + 1}").ToList() };
            }
        }
    }
}
