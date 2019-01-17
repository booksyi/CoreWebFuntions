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
    public class CombineJson
    {
        public class Request : IRequest<Response>
        {
        }

        public class Row
        {
            public int id { get; set; }
            public string project_country { get; set; }
            public string project_date { get; set; }
            public string executing_agency { get; set; }
            public string contractor_name { get; set; }
            public string contractor_address { get; set; }
            public decimal total_contract_amount { get; set; }
            public decimal financed_by_adb { get; set; }
        }

        public class Response : MultiResponse<Row>
        {
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly Random random = new Random();

            public Handler()
            {
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                List<Row> rows = new List<Row>();
                for (int i = 1; i <= 177; i++)
                {
                    //string path = $@"‪D:\temp\output\finish\page-{i}.txt";
                    string path = $@"‪\..\..\..\..\temp\output\finish\page-{i}.txt";
                    string json = await File.ReadAllTextAsync(path);
                    rows.AddRange(JsonConvert.DeserializeObject<Row[]>(json));
                }
                await File.WriteAllTextAsync($@"‪\..\..\..\..\temp\output\combine.txt", JsonConvert.SerializeObject(rows));
                return new Response() { Result = rows };
            }
        }
    }
}
