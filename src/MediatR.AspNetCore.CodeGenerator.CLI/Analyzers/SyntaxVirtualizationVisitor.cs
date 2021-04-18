namespace MediatR.AspNetCore.CodeGenerator.CLI.Analyzers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    class SyntaxVirtualizationVisitor : CSharpSyntaxRewriter
    {
        public List<TypeDeclarationSyntax> Requests { get; private set; } = new();

        public override SyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax syntaxNode)
        {
            this.VisitDeclaration(syntaxNode);
            return base.VisitRecordDeclaration(syntaxNode);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax syntaxNode)
        {
            this.VisitDeclaration(syntaxNode);
            return base.VisitClassDeclaration(syntaxNode);
        }

        private void VisitDeclaration(TypeDeclarationSyntax syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax
             || syntaxNode is RecordDeclarationSyntax)
            {
                if (syntaxNode.BaseList == null)
                    return;

                var baselist = syntaxNode.BaseList;
                foreach (var entry in baselist.Types)
                {
                    if (entry is SimpleBaseTypeSyntax basetype
                     && basetype.Type is GenericNameSyntax type)
                    {
                        if (type.Identifier.ValueText == "IRequest"
                         && type.TypeArgumentList.Arguments.Count == 1
                         && type.TypeArgumentList.Arguments[0] is TypeSyntax)
                        {
                            Requests.Add(syntaxNode);
                        }
                    }
                }
            }
        }
    }
}
