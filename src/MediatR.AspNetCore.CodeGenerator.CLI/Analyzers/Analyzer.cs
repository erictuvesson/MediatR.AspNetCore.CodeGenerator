namespace MediatR.AspNetCore.CodeGenerator.CLI.Analyzers
{
    using MediatR.AspNetCore.CodeGenerator.CLI.Analyzers.Objects;
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;
    using System.Linq;

    public static class Analyzer
    {
        public static List<Request> GetRequests(this Compilation compilation)
        {
            var visitor = new SyntaxVirtualizationVisitor();
            foreach (var syntaxTree in compilation.SyntaxTrees)
                visitor.Visit(syntaxTree.GetRoot());

            return visitor.Requests
                .Select(x => new Request(x, compilation))
                .ToList();
        }
    }
}
