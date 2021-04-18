namespace MediatR.AspNetCore.CodeGenerator.CLI.Analyzers.Objects
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Seperate the syntax code from the view code.
    /// Makes it easier to debug the code. :)
    /// </summary>
    public class RequestSyntax
    {
        static readonly string DisplayAttribute = "System.ComponentModel.DataAnnotations.DisplayAttribute";

        /// <summary>
        /// Gets the request syntax.
        /// </summary>
        public TypeDeclarationSyntax Syntax { get; }

        /// <summary>
        /// Gets the request return syntax.
        /// </summary>
        public TypeSyntax ReturnTypeSyntax { get; }

        /// <summary>
        /// Gets the request namespace.
        /// </summary>
        public NamespaceDeclarationSyntax NamespaceSyntax { get; }

        /// <summary>
        /// Gets the request return type namespace.
        /// </summary>
        public INamespaceSymbol ReturnTypeNamespaceSyntax { get; }

        /// <summary>
        /// Gets meta tags added by <see cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/>.
        /// </summary>
        public Dictionary<string, string> Meta { get; }

        public RequestSyntax(TypeDeclarationSyntax syntax, Compilation compilation)
        {
            this.Syntax = syntax;
            this.ReturnTypeSyntax = GetRequestReturnType(syntax);
            this.NamespaceSyntax = GetRelativeNamespaceSyntax(this.Syntax);

            var semanticModel = compilation.GetSemanticModel(this.NamespaceSyntax?.SyntaxTree ?? syntax.SyntaxTree);
            this.ReturnTypeNamespaceSyntax = GetNamespace(this.ReturnTypeSyntax, semanticModel);

            if (this.Syntax.AttributeLists.Count > 0)
            {
                // this is a little overkill, and is probably slow?
                var attr = GetAttributes(this.Syntax, "Display")
                    .FirstOrDefault(x => semanticModel.GetTypeInfo(x).Type?.OriginalDefinition.ToString() == DisplayAttribute);

                this.Meta = attr.ArgumentList.Arguments
                    .Where(x => x.NameEquals != null)
                    .ToDictionary(x => x.NameEquals.Name.ToString(), x => x.Expression.ToString());
            }
        }

        private static IEnumerable<AttributeSyntax> GetAttributes(TypeDeclarationSyntax syntax, string attrName)
        {
            return syntax.AttributeLists
                .SelectMany(x => x.Attributes
                    .Where(a => a.Name.ToString().Contains(attrName))
                );
        }

        private static TypeSyntax GetRequestReturnType(TypeDeclarationSyntax syntax)
        {
            foreach (var entry in syntax.BaseList.Types)
            {
                if (entry is SimpleBaseTypeSyntax basetype)
                {
                    if (basetype.Type is GenericNameSyntax type)
                    {
                        if (type.Identifier.ValueText == "IRequest"
                         && type.TypeArgumentList.Arguments.Count == 1
                         && type.TypeArgumentList.Arguments[0] is TypeSyntax)
                        {
                            return type.TypeArgumentList.Arguments[0];
                        }
                    }
                }
            }
            return null;
        }

        private static NamespaceDeclarationSyntax GetRelativeNamespaceSyntax(SyntaxNode node, int maxDepth = 8)
        {
            SyntaxNode current = node;
            for (int i = 0; i < maxDepth; i++)
            {
                if (current is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
                    return namespaceDeclarationSyntax;
                current = node.Parent;
            }
            return null;
        }

        private static INamespaceSymbol GetNamespace(SyntaxNode node, SemanticModel semanticModel)
        {
            return semanticModel.GetTypeInfo(node).Type?.ContainingNamespace;
        }
    }
}
