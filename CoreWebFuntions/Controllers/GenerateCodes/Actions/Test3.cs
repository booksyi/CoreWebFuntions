using AutoMapper;
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
    public class Test3
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
                CsSchema.Class csClass = new CsSchema.Class("Project");
                csClass.Access = CsSchema.Access.Public;
                csClass.Fields = new CsSchema.Field[]
                {
                    new CsSchema.Field("bool", "isAA")
                };
                csClass.Properties = new CsSchema.Property[]
                {
                    new CsSchema.Property("string", "ppp1")
                    {
                        Attributes = new CsSchema.Attribute[]
                        {
                            new CsSchema.Attribute("Key"),
                            new CsSchema.Attribute("Column")
                        }
                    }
                };
                csClass.Attributes = new CsSchema.Attribute[]
                {
                    new CsSchema.Attribute("Table", "C1", "QQ = \"X\"", "Z = 123")
                };

                CsSchema.Unit unit = new CsSchema.Unit();
                CsSchema.Namespace @namespace = new CsSchema.Namespace();
                @namespace.Name = "TestProject";
                @namespace.Classes = new CsSchema.Class[] { csClass };
                unit.Usings = new string[]
                {
                    "System",
                    "System.Collections.Generic"
                };
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
