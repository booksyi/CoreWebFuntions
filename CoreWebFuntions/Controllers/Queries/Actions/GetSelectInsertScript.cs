using AutoMapper;
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
    public class GetSelectInsertScript
    {
        public class Request : IRequest<Response>
        {
            internal string TableName { get; set; }
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
            private readonly IMapper mapper;

            public Handler(IOptions<DatabaseConfig> databaseConfig, SqlHelper sqlHelper, IMapper mapper)
            {
                this.databaseConfig = databaseConfig.Value;
                this.sqlHelper = sqlHelper;
                this.mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                List<string> lines = new List<string>();
                DbSchema.Table tableSchema = CodingHelper.GetDbTableSchema(databaseConfig.ConnectionString, request.TableName);

                if (request.ContainsIdentity && tableSchema.Identity != null)
                {
                    lines.Add($"SELECT 'SET IDENTITY_INSERT [{tableSchema.Name}] ON'");
                }

                List<string> fields = new List<string>();
                List<string> values = new List<string>();
                foreach (var field in tableSchema.Fields)
                {
                    var csProperty = mapper.Map<CsSchema.Property>(field);

                    if (field.IsIdentity && (request.ContainsIdentity == false))
                    {
                        continue;
                    }
                    fields.Add($"[{field.Name}]");

                    string value = $"[{field.Name}]";
                    if (csProperty.TypeName != "string")
                    {
                        value = $"CAST({value} AS NVARCHAR)";
                    }
                    if (csProperty.TypeName == "string")
                    {
                        value = $"({value} COLLATE Chinese_Taiwan_Stroke_CI_AS)";
                    }
                    if (csProperty.TypeName.In("DateTime", "DateTime?"))
                    {
                        value = $"'''' + {value} + ''''";
                    }
                    if (csProperty.TypeName == "string")
                    {
                        value = $"'''' + REPLACE({value}, '''', '''''') + ''''";
                    }
                    if (field.IsNullable)
                    {
                        value = $"ISNULL({value}, 'null')";
                    }
                    values.Add(value);
                }

                string sql = $@"
                    SELECT '
                    INSERT INTO [{tableSchema.Name}] ({string.Join(", ", fields)})
                    VALUES (' + {string.Join(" + ', ' + ", values)} + ');'
                    FROM [{tableSchema.Name}]".DecreaseIndent();
                lines.Add(sql);

                if (request.ContainsIdentity && tableSchema.Identity != null)
                {
                    lines.Add($"SELECT 'SET IDENTITY_INSERT [{tableSchema.Name}] OFF'");
                }

                return new Response() { Lines = lines };
            }
        }
    }
}
