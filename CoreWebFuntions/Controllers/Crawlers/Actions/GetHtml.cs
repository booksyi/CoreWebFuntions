using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Crawlers.Actions
{
    public class GetHtml
    {
        public class Request : IRequest<uint>
        {
            public int PageFrom { get; set; } = 1;
        }

        public class Handler : IRequestHandler<Request, uint>
        {
            private readonly Random random = new Random();

            public Handler()
            {
            }

            public async Task<uint> Handle(Request request, CancellationToken token)
            {
                string saveHtmlPath = @"D:\temp\output\adbGoods";
                string urlFormat = $"{"https:"}//www.adb.org/projects/tenders/status/awarded-1586/group/goods?page={{0}}";

                int pageFrom = request.PageFrom;
                int pageMax = 117;

                for (int round = 1; round <= 12; round++)
                {
                    int from = Math.Max((round - 1) * 10 + 1, pageFrom);
                    int max = Math.Min(from + 9, pageMax);

                    for (int page = from; page <= max; page++)
                    {
                        string url = string.Format(urlFormat, page - 1);
                        using (Process myProcess = new Process())
                        {
                            myProcess.StartInfo.UseShellExecute = true;
                            myProcess.StartInfo.FileName = url;
                            myProcess.Start();
                        }
                        await Task.Delay(300);
                    }
                    await Task.Delay(13000);
                    for (int page = from; page <= max; page++)
                    {
                        string url = string.Format(urlFormat, page - 1);
                        string html = await GetHtml(url);
                        await File.WriteAllTextAsync(Path.Combine(saveHtmlPath, $"page-{page}.html"), html);
                        await Task.Delay(300);
                    }
                }
                return 0;
            }

            public async Task<string> GetHtml(string url)
            {
                string html = null;
                using (HttpClient client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Add("Set-Cookie", string.Format("{0}={1}; path=/", cookie.Name, cookie.Value));
                    int retry = 3;
                    while (html == null && retry > 0)
                    {
                        using (var message = await client.GetAsync(url))
                        {
                            if (message.StatusCode == HttpStatusCode.OK)
                            {
                                html = await message.Content.ReadAsStringAsync();
                            }
                            else
                            {
                                retry = retry - 1;
                                if (retry == 0)
                                {
                                    // write log: failed
                                }
                                else
                                {
                                    url = $"{url}&{retry}";
                                    await Task.Delay(10000);
                                }
                            }
                        }
                    }
                }
                await Task.Delay(500);
                return html;
            }
        }
    }
}
