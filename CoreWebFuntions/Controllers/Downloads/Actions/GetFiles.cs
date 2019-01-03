using CoreWebFuntions.Data.Configs;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Downloads.Actions
{
    public class GetFiles
    {
        public class Request : IRequest<Response>
        {
        }

        public class Row
        {
            public string Key { get; set; }
            public string Path { get; set; }
            public DateTime LastWriteTime { get; set; }
        }

        public class Response
        {
            public IEnumerable<Row> Rows { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DownloadConfig downloadConfig;

            public Handler(IOptions<DownloadConfig> downloadConfig)
            {
                this.downloadConfig = downloadConfig.Value;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                List<Row> rows = new List<Row>();
                foreach (var file in downloadConfig.Files)
                {
                    if (File.Exists(file.Path))
                    {
                        FileInfo fileInfo = new FileInfo(file.Path);
                        rows.Add(new Row() { Key = file.Key, Path = file.Path, LastWriteTime = fileInfo.LastWriteTime });
                    }
                    else if (Directory.Exists(file.Path))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(file.Path);
                        rows.Add(new Row() { Key = file.Key, Path = file.Path, LastWriteTime = directoryInfo.LastWriteTime });
                    }
                }
                return new Response() { Rows = rows };
            }
        }
    }
}
