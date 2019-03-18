using CoreWebFuntions.Data;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.GenerateCodes.Actions
{
    public class Test1
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

            public Handler(DatabaseContext context)
            {
                this.context = context;
            }

            public class Property
            {
                public string TypeName { get; set; }
                public string Name { get; set; }
                public Property(string type, string name)
                {
                    TypeName = type;
                    Name = name;
                }
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                string[] usings = new string[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Linq"
                };

                string @namespace = "CoreWebFuntions.Data";
                string @class = "Class1";
                string[] bases = null;
                Property[] properties = new Property[]
                {
                    new Property("int", "A1"),
                    new Property("int?", "A2"),
                };


                var syntaxFactory = SyntaxFactory.CompilationUnit();
                // add using
                foreach (string @using in usings)
                {
                    syntaxFactory = syntaxFactory.AddUsings(
                        SyntaxFactory.UsingDirective(
                            SyntaxFactory.ParseName(@using)));
                }
                // namespace
                var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(
                    SyntaxFactory.ParseName(@namespace)).NormalizeWhitespace();
                // class
                var classDeclaration = SyntaxFactory.ClassDeclaration(@class)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                // class : base
                if (bases != null)
                {
                    foreach (string @base in bases)
                    {
                        classDeclaration = classDeclaration.AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(@base)));
                    }
                }


                foreach (Property property in properties)
                {
                    var propertyDeclaration = SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName(property.TypeName), property.Name)
                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                            .AddAccessorListAccessors(
                                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                    classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
                }

                namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);
                syntaxFactory = syntaxFactory.AddMembers(namespaceDeclaration);

                // Normalize and get code as string.
                var code = syntaxFactory
                    .NormalizeWhitespace()
                    .ToFullString();

                // Output new code to the console.
                return new Response
                {
                    Code = code
                };
            }
        }
    }
}
