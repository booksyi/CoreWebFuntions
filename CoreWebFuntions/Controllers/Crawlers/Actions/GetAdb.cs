using HelpersForCore;
using HtmlAgilityPack;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Crawlers.Actions
{
    public class GetAdb
    {
        public class Request : IRequest<Response>
        {
            public int PageFrom { get; set; } = 1;
        }

        public class OriginRow
        {
            public string Summary { get; set; }
            public string ExecutingAgency { get; set; }
            public string ContractorName { get; set; }
            public string ContractorAddress { get; set; }
            public string TotalContractAmount { get; set; }
            public string FinancedByAdb { get; set; }
        }

        public class ResultRow
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

        public class Response : MultiResponse<ResultRow>
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
                List<ResultRow> resultRows = new List<ResultRow>();

                bool @continue = true;
                int pageFrom = request.PageFrom;
                int pageSize = 20;
                int pageFinal = 0;
                for (int i = pageFrom; @continue; i++)
                {
                    List<OriginRow> originRows = new List<OriginRow>();
                    string url = $"{"https:"}//www.adb.org/projects/tenders/status/awarded-1586?page={i - 1}";
                    @continue = await CrawlerMaster(originRows, url);

                    for (int j = 0; j < originRows.Count; j++)
                    {
                        string[] summaries = originRows[j].Summary.Split(";");
                        int summariesDateIndex = Array.IndexOf(summaries, summaries.FirstOrDefault(x => x.Trim().StartsWith("Contract date:")));
                        resultRows.Add(new ResultRow()
                        {
                            id = (i - 1) * pageSize + j,
                            project_country = summaries[1],
                            sector =
                                summariesDateIndex >= 0 ?
                                    (summaries.Length > summariesDateIndex - 1 ? summaries[summariesDateIndex - 1] : null) :
                                    (summaries.Length >= 3 ? summaries[2] : null),
                            project_date = ConvertToDateString(summaries.FirstOrDefault(x => x.Trim().StartsWith("Contract date:")).Replace("Contract date:", "").Trim()),
                            executing_agency = originRows[j].ExecutingAgency,
                            contractor_name = originRows[j].ContractorName,
                            contractor_address = originRows[j].ContractorAddress,
                            total_contract_amount = originRows[j].TotalContractAmount != null ? Convert.ToDecimal(originRows[j].TotalContractAmount.Replace(",", "")) : 0,
                            financed_by_adb = originRows[j].FinancedByAdb != null ? Convert.ToDecimal(originRows[j].FinancedByAdb.Replace(",", "")) : 0
                        });
                    }
                    pageFinal = i;
                    await System.IO.File.WriteAllTextAsync($@"D:\temp\output\page-{i}.txt", Newtonsoft.Json.JsonConvert.SerializeObject(resultRows.Skip((i - pageFrom) * pageSize)));
                }
                await System.IO.File.WriteAllTextAsync($@"D:\temp\output\page-from-{pageFrom}-to-{pageFinal - 1}.txt", Newtonsoft.Json.JsonConvert.SerializeObject(resultRows));

                return new Response() { Result = resultRows };
            }

            public async Task<bool> CrawlerMaster(List<OriginRow> rows, string url)
            {
                string html = await GetHtml(url);
                if (html == null)
                {
                    return false;
                }
                string fileName = $"{url.Substring(url.IndexOf("?") + 1)}.html";
                string savePath = System.IO.Path.Combine(@"D:\temp\output\html", fileName);
                var saveTask = System.IO.File.WriteAllTextAsync(savePath, html);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode root = doc.DocumentNode;
                HtmlNodeCollection items = root.SelectNodes("//article/div[@class='list']/div[@class='item']");
                if (items != null && items.Any())
                {
                    foreach (HtmlNode item in items)
                    {
                        OriginRow row = new OriginRow();
                        row.Summary = WebUtility.HtmlDecode(item.SelectSingleNode("./div[@class='item-summary']").InnerText).Trim();
                        HtmlNodeCollection details = item.SelectNodes("./div[@class='item-details']/p");
                        foreach (HtmlNode detail in details)
                        {
                            string key = detail.SelectSingleNode("./span[1]").InnerText.Trim();
                            string value = detail.SelectSingleNode("./span[2]").InnerText.Trim();
                            switch (key)
                            {
                                case "Notice Type:":
                                    break;
                                case "Approval Number:":
                                    break;
                                case "Executing Agency:":
                                    row.ExecutingAgency = value;
                                    break;
                                case "Contractor Name:":
                                    row.ContractorName = value;
                                    break;
                                case "Address:":
                                    row.ContractorAddress = value;
                                    break;
                                case "Total Contract Amount (US$):":
                                    row.TotalContractAmount = value;
                                    break;
                                case "Contract Amount Financed by ADB (US$):":
                                    row.FinancedByAdb = value;
                                    break;
                            }
                        }
                        rows.Add(row);
                    }
                    return true;
                }
                return false;
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
                        using (Process myProcess = new Process())
                        {
                            myProcess.StartInfo.UseShellExecute = true;
                            myProcess.StartInfo.FileName = url;
                            myProcess.Start();
                            await Task.Delay(6000);
                        }

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

            public string ConvertToDateString(string origin)
            {
                if (string.IsNullOrWhiteSpace(origin))
                {
                    return null;
                }
                if (origin == "Not available")
                {
                    return null;
                }

                string[] values = origin.Split(' ');
                return $"{values[2]}-{months[values[1]]}-{values[0]}";
            }

            static Dictionary<string, string> months = new Dictionary<string, string>()
            {
                { "Jan", "01" },
                { "Feb", "02" },
                { "Mar", "03" },
                { "Apr", "04" },
                { "May", "05" },
                { "Jun", "06" },
                { "Jul", "07" },
                { "Aug", "08" },
                { "Sep", "09" },
                { "Oct", "10" },
                { "Nov", "11" },
                { "Dec", "12" },
            };
        }
    }
}
