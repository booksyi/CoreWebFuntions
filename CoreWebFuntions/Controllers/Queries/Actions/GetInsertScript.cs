using CoreWebFuntions.Data;
using CoreWebFuntions.Data.Configs;
using HelpersForCore;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Queries.Actions
{
    /// <summary>
    /// 將指定 Table 的資料以 Insert 指令傳回
    /// </summary>
    public class GetInsertScript
    {
        public class Request : IRequest<Response>
        {
            internal string TableName { get; set; }
            public int StartRowNumber { get; set; } = 1;
            public int EndRowNumber { get; set; } = 5000;
            public bool ContainsIdentity { get; set; } = true;
        }

        public class Response
        {
            public IEnumerable<string> Lines { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DatabaseConfig databaseConfig;
            private readonly SqlHelper sqlHelper;

            public Handler(IOptions<DatabaseConfig> databaseConfig, SqlHelper sqlHelper)
            {
                this.databaseConfig = databaseConfig.Value;
                this.sqlHelper = sqlHelper;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                List<string> lines = new List<string>();
                DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(databaseConfig.ConnectionString, request.TableName);

                if (request.ContainsIdentity && tableSchema.Identity != null)
                {
                    lines.Add($"SET IDENTITY_INSERT [{tableSchema.TableName}] ON");
                }

                string sql = @"
                    SELECT $Fields 
                    FROM   (SELECT Row_number() 
                                     OVER(ORDER BY [$OrderBy]) AS [__RowNumber], * 
                            FROM   [$TableName]) t 
                    WHERE  [__RowNumber] BETWEEN @Start AND @End 
                    ORDER  BY [__RowNumber] ";

                string orderby = null;
                if (tableSchema.Identity != null)
                {
                    orderby = tableSchema.Identity.Name;
                }
                else if (tableSchema.PrimaryKeys.Any())
                {
                    orderby = tableSchema.PrimaryKeys.First().Name;
                }
                else if (tableSchema.Fields.Any(x => x.ForCs.TypeName == "int"))
                {
                    orderby = tableSchema.Fields.First(x => x.ForCs.TypeName == "int").Name;
                }
                else
                {
                    orderby = tableSchema.Fields.FirstOrDefault().Name;
                }

                sql = sql.Replace("$Fields", string.Join(", ", tableSchema.Fields.Select(x => $"[{x.Name}]")));
                sql = sql.Replace("$OrderBy", orderby);
                sql = sql.Replace("$TableName", tableSchema.TableName);

                await sqlHelper.ExecuteReaderEachAsync(sql,
                    new SqlParameter[]
                    {
                        new SqlParameter("@Start", request.StartRowNumber),
                        new SqlParameter("@End", request.EndRowNumber)
                    },
                    reader =>
                    {
                        List<string> fields = new List<string>();
                        List<string> values = new List<string>();
                        foreach (var field in tableSchema.Fields)
                        {
                            if (field.IsIdentity && (request.ContainsIdentity == false))
                            {
                                continue;
                            }
                            fields.Add($"[{field.Name}]");
                            if (reader[field.Name] == DBNull.Value || reader[field.Name] == null)
                            {
                                values.Add("null");
                            }
                            else if (field.TypeName.In("bigint", "decimal", "float", "int", "money", "numeric", "real", "smallint", "smallmoney"))
                            {
                                values.Add($"{reader[field.Name]}");
                            }
                            else if (field.TypeName.In("date"))
                            {
                                values.Add($"'{reader[field.Name]:yyyy/MM/dd}'");
                            }
                            else if (field.TypeName.In("datetime"))
                            {
                                values.Add($"'{reader[field.Name]:yyyy/MM/dd HH:mm:ss}'");
                            }
                            else
                            {
                                string value = Convert.ToString(reader[field.Name]).Replace("'", "''");
                                values.Add($"'{value}'");
                            }
                        }
                        lines.Add($"INSERT INTO [{tableSchema.TableName}] ({string.Join(", ", fields)}) VALUES ({string.Join(", ", values)});");
                    });

                if (request.ContainsIdentity && tableSchema.Identity != null)
                {
                    lines.Add($"SET IDENTITY_INSERT [{tableSchema.TableName}] OFF");
                }

                return new Response() { Lines = lines };
            }
        }
    }
}
