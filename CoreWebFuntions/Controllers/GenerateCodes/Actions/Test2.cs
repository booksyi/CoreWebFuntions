﻿using AutoMapper;
using CoreWebFuntions.Data;
using HelpersForCore;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.GenerateCodes.Actions
{
    public class Test2
    {
        public class Request : IRequest<Response>
        {
        }

        public class Response
        {
            public string Code { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DatabaseContext context;
            private readonly IMapper mapper;

            public Handler(DatabaseContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                string connectionString = "Server=LAPTOP-RD9P71LP\\SQLEXPRESS;Database=TEST1;UID=sa;PWD=1234;";

                DbSchema.Table tableSchema = CodingHelper.GetDbTableSchema(connectionString, "Article");
                CsSchema.Class csClass = mapper.Map<CsSchema.Class>(tableSchema);

                CsSchema.Unit unit = new CsSchema.Unit();
                CsSchema.Namespace @namespace = new CsSchema.Namespace();
                @namespace.Name = "QQ";
                @namespace.Classes = new CsSchema.Class[] { csClass };
                unit.Namespaces = new CsSchema.Namespace[] { @namespace };
                var syntax = mapper.Map<CompilationUnitSyntax>(unit);

                return new Response
                {
                    Code = syntax
                        .NormalizeWhitespace()
                        .ToFullString()
                };
            }
        }
    }
}
