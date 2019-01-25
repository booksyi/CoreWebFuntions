using HelpersForCore;
using HtmlAgilityPack;
using MediatR;
using Newtonsoft.Json;
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
    public class GetAdb2
    {
        public class Request : IRequest<Response>
        {
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
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }

            [JsonProperty(PropertyName = "group")]
            public string Group { get; set; }

            [JsonProperty(PropertyName = "project-country")]
            public string ProjectCountry { get; set; }

            [JsonProperty(PropertyName = "sector")]
            public string Sector { get; set; }

            [JsonProperty(PropertyName = "project-date")]
            public string ProjectDate { get; set; }

            [JsonProperty(PropertyName = "executing-agency")]
            public string ExecutingAgency { get; set; }

            [JsonProperty(PropertyName = "contractor-name")]
            public string ContractorName { get; set; }

            [JsonProperty(PropertyName = "contractor-country")]
            public string ContractorCountry { get; set; }

            [JsonProperty(PropertyName = "total-contract-amount")]
            public decimal TotalContractAmount { get; set; }

            [JsonProperty(PropertyName = "financed-by-adb")]
            public decimal FinancedByAdb { get; set; }
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

                string saveHtmlPath = @"D:\temp\output\adbGoodsConsulting";

                int pageFrom = 1;
                int pageMax = 177;
                int pageSize = 20;

                for (int page = pageFrom; page <= pageMax; page++)
                {
                    string path = Path.Combine(saveHtmlPath, $"page-{page}.html");
                    var originRows = await CrawlerMaster(path);
                    int count = originRows.Count();
                    for (int j = 0; j < originRows.Count(); j++)
                    {
                        var originRow = originRows.ElementAt(j);
                        string[] summaries = originRow.Summary.Split(";");
                        var row = new ResultRow()
                        {
                            Id = (page - 1) * pageSize + j + 1,
                            Group = "Consulting",
                            ProjectCountry = ConvertCountryString(summaries[1]),
                            ExecutingAgency = originRow.ExecutingAgency,
                            ContractorName = originRow.ContractorName,
                            ContractorCountry = ConvertCountryString(originRow.ContractorAddress),
                            TotalContractAmount = originRow.TotalContractAmount != null ? Convert.ToDecimal(originRow.TotalContractAmount.Replace(",", "")) : 0,
                            FinancedByAdb = originRow.FinancedByAdb != null ? Convert.ToDecimal(originRow.FinancedByAdb.Replace(",", "")) : 0
                        };
                        if (summaries.Count() == 3)
                        {
                            if (summaries[2].Trim().StartsWith("Contract date:") == false)
                            {
                                row.ProjectDate = ConvertToDateString(summaries[2].Replace("Contract date:", "").Trim());
                            }
                            else
                            {
                                row.Sector = summaries[2].Trim();
                            }
                        }
                        else if (summaries.Count() > 3)
                        {
                            row.Sector = summaries[2].Trim();
                            row.ProjectDate = ConvertToDateString(summaries[3].Replace("Contract date:", "").Trim());
                        }
                        resultRows.Add(row);
                    }
                }

                await File.WriteAllTextAsync(@"D:\temp\output\result.txt", JsonConvert.SerializeObject(resultRows));
                return new Response() { Result = resultRows };
            }

            public async Task<IEnumerable<OriginRow>> CrawlerMaster(string path)
            {
                List<OriginRow> rows = new List<OriginRow>();
                string html = await File.ReadAllTextAsync(path);

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
                }
                return rows;
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

            public string ConvertCountryString(string origin)
            {
                if (origin == null)
                {
                    return null;
                }
                foreach (var country in countries)
                {
                    if (origin.ToLower().Contains(country.ToLower()))
                    {
                        return country;
                    }
                }
                return origin;
            }

            static string[] countries = {
                "Afghanistan",
                "Armenia",
                "Australia",
                "Austria",
                "Azerbaijan",
                "Bangladesh",
                "Belgium",
                "Bhutan",
                "Cambodia",
                "Canada",
                "China",
                "Cook Islands",
                "Denmark",
                "Fiji",
                "Finland",
                "France",
                "Georgia",
                "Germany",
                "Hong Kong",
                "India",
                "Indonesia",
                "Ireland",
                "Italy",
                "Japan",
                "Kazakhstan",
                "Kiribati",
                "Korea",
                "Kyrgyz Republic",
                "Lao People's Democratic",
                "Malaysia",
                "Maldives",
                "Marshall Islands",
                "Micronesia",
                "Mongolia",
                "Myanmar",
                "Nauru",
                "Nepal",
                "Netherland",
                "New Zealand",
                "Pakistan",
                "Palau",
                "Papua New Guinea",
                "Philippines",
                "Portugal",
                "Samoa",
                "Singapore",
                "Solomon Islands",
                "Korea",
                "Spain",
                "Sri Lanka",
                "Sweden",
                "Switzerland",
                "Tajikistan",
                "Thailand",
                "Timor-Leste",
                "Tonga",
                "Turkey",
                "Tuvala",
                "United Kingdom",
                "United States",
                "Uzbekistan",
                "Vanuatu",
                "Viet Nam",
                "Various",
                "Regional",
                "UAE"
            };
        }
    }
}
