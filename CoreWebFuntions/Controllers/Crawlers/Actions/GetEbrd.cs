using HelpersForCore;
using HtmlAgilityPack;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Crawlers.Actions
{
    public class GetEbrd
    {
        public class Request : IRequest<Response>
        {
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

        public class ResultRow
        {
            public int project_id { get; set; }
            public string project_name { get; set; }
            public string project_url { get; set; }
            public string project_number { get; set; }
            public string procurement_ref_number { get; set; }
            public string location { get; set; }
            public string business_sector { get; set; }
            public string funding_source { get; set; }
            public string contract_type { get; set; }
            public string notice_type { get; set; }
            public string issue_date { get; set; }
            public string closing_date { get; set; }
        }

        public class OriginRow
        {
            public string IssueDate { get; set; }
            public string ClosingDate { get; set; }
            public string Location { get; set; }
            public string ProjectName { get; set; }
            public string ProjectLink { get; set; }
            public Detail ProjectDetail { get; set; }
            public string Sector { get; set; }
            public string Contract { get; set; }
            public string Type { get; set; }
        }

        public class Detail
        {
            public string ProcurementRef { get; set; }
            public string Location { get; set; }
            public string BusinessSector { get; set; }
            public string ProjectNumber { get; set; }
            public string FundingSource { get; set; }
            public string ContractType { get; set; }
            public string NoticeType { get; set; }
            public string IssueDate { get; set; }
            public string ClosingDate { get; set; }

            public string Context { get; set; }
        }

        public class Response : MultiResponse<ResultRow>
        {
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Handler()
            {
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                List<ResultRow> resultRows = new List<ResultRow>();

                bool @continue = true;
                int beginPage = 11;
                for (int i = beginPage; @continue; i++)
                {
                    List<OriginRow> originRows = new List<OriginRow>();
                    string url = $"{"https:"}//www.ebrd.com/cs/Satellite?c=Page&cid=1395238128830&ct0=on&ct1=on&ct2=on&d=&pagename=EBRD/Page/SearchAndFilterProcurement&page={i}&safSortBy=Title&safSortOrder=ascending";
                    @continue = await CrawlerMaster(originRows, url);

                    for (int j = 0; j < originRows.Count; j++)
                    {
                        resultRows.Add(new ResultRow()
                        {
                            project_id = (i - 1) * 10 + j,
                            project_name = originRows[j].ProjectName,
                            project_url = originRows[j].ProjectLink,
                            project_number = originRows[j].ProjectDetail.ProjectNumber,
                            procurement_ref_number = originRows[j].ProjectDetail.ProcurementRef,
                            location = originRows[j].ProjectDetail.Location,
                            business_sector = originRows[j].ProjectDetail.BusinessSector,
                            funding_source = originRows[j].ProjectDetail.FundingSource,
                            contract_type = originRows[j].ProjectDetail.ContractType,
                            notice_type = originRows[j].ProjectDetail.NoticeType,
                            issue_date = ConvertToDateString(originRows[j].IssueDate),
                            closing_date = ConvertToDateString(originRows[j].ClosingDate)
                        });
                    }
                    await System.IO.File.WriteAllTextAsync($@"D:\temp\output\page-{i}.txt", Newtonsoft.Json.JsonConvert.SerializeObject(resultRows.Skip((i - beginPage) * 10)));
                }
                await System.IO.File.WriteAllTextAsync($@"D:\temp\output\page-from-{beginPage}.txt", Newtonsoft.Json.JsonConvert.SerializeObject(resultRows));

                return new Response() { Result = resultRows };
            }

            public async Task<bool> CrawlerMaster(List<OriginRow> rows, string url)
            {
                string html = await GetHtml(url);
                if (html == null)
                {
                    return false;
                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode root = doc.DocumentNode;
                HtmlNodeCollection items = root.SelectNodes("//tbody[@id='posts']/tr");
                if (items != null && items.Any())
                {
                    foreach (HtmlNode item in items)
                    {
                        OriginRow row = new OriginRow();
                        row.IssueDate = item.SelectSingleNode("./td[1]").InnerText.Trim();
                        row.ClosingDate = item.SelectSingleNode("./td[2]").InnerText.Trim();
                        row.Location = item.SelectSingleNode("./td[3]").InnerText.Trim();
                        row.ProjectName = item.SelectSingleNode("./td[4]/a").InnerText.Trim();
                        string href = WebUtility.HtmlDecode(item.SelectSingleNode("./td[4]/a").Attributes["href"].Value);
                        if (href.StartsWith("//"))
                        {
                            row.ProjectLink = $"https:{href}";
                        }
                        else if (href.StartsWith("/"))
                        {
                            row.ProjectLink = $"https://www.ebrd.com{href}";
                        }
                        else if (href.StartsWith("http:") || href.StartsWith("https:"))
                        {
                            row.ProjectLink = href;
                        }
                        else
                        {
                            row.ProjectLink = $"https://www.ebrd.com/{href}";
                        }
                        row.ProjectDetail = await CrawlerDetail(row.ProjectLink);
                        row.Sector = item.SelectSingleNode("./td[5]").InnerText.Trim();
                        row.Contract = item.SelectSingleNode("./td[6]").InnerText.Trim();
                        row.Type = item.SelectSingleNode("./td[7]").InnerText.Trim();
                        rows.Add(row);
                    }
                    return true;
                }
                return false;
            }

            public async Task<Detail> CrawlerDetail(string url)
            {
                string html = await GetHtml(url);
                if (html == null)
                {
                    return null;
                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode root = doc.DocumentNode;
                HtmlNode item = root.SelectSingleNode("//div[@class='psd-highlights']");
                if (item != null)
                {
                    Detail detail = new Detail();
                    int count = item.SelectNodes(".//legend").Count;
                    for (int i = 1; i <= count; i++)
                    {
                        string legend = item.SelectSingleNode($".//legend[{i}]").InnerText.Trim();
                        switch (legend)
                        {
                            case "Procurement ref:":
                                detail.ProcurementRef = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Location:":
                                detail.Location = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Business sector:":
                                detail.BusinessSector = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Project number:":
                                detail.ProjectNumber = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Funding source:":
                                detail.FundingSource = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Contract type:":
                                detail.ContractType = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Notice type:":
                                detail.NoticeType = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Issue date:":
                                detail.IssueDate = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                            case "Closing date:":
                                detail.ClosingDate = item.SelectSingleNode($".//p[{i}]").InnerText.Trim();
                                break;
                        }
                    }
                    detail.Context = root.SelectSingleNode("//article").InnerHtml;
                    return detail;
                }
                return null;
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
                                    await Task.Delay(15000);
                                }
                            }
                        }
                    }
                }
                await Task.Delay(300);
                return html;
            }

            public string ConvertToDateString(string origin)
            {
                if (string.IsNullOrWhiteSpace(origin))
                {
                    return null;
                }

                string[] values = origin.Split(' ');
                return $"{values[2]}-{months[values[1]]}-{values[0]}";
            }
        }
    }
}
