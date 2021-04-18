namespace MediatR.AspNetCore.CodeGenerator.CLI.Analyzers.Objects
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class Request
    {
        static readonly string DefaultGroupName = "MediatR";

        /// <summary>
        /// Gets the request syntax information.
        /// </summary>
        public RequestSyntax Syntax { get; }

        public string GroupName { get; }

        /// <summary>
        /// Gets the request trivia.
        /// </summary>
        public string Trivia { get; }

        /// <summary>
        /// Gets the request full name.
        /// </summary>
        public string FullName => $"{Namespace}.{Name}";

        /// <summary>
        /// Gets the request name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the request namespace.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Gets the request full return type.
        /// </summary>
        public string FullReturnType => CalcFullReturnType();

        /// <summary>
        /// Gets the request return type.
        /// </summary>
        public string ReturnType { get; }

        /// <summary>
        /// Gets the request return namespace.
        /// </summary>
        public string ReturnNamespace { get; }

        public Request(TypeDeclarationSyntax syntax, Compilation compilation)
        {
            this.Syntax = new RequestSyntax(syntax, compilation);

            this.Trivia = syntax.GetLeadingTrivia().ToString().Replace("    ", "        ");
            this.Name = syntax.Identifier.ValueText;
            this.ReturnType = this.Syntax.ReturnTypeSyntax.ToString();
            this.Namespace = this.Syntax.NamespaceSyntax.Name.ToString();
            this.ReturnNamespace = this.Syntax.ReturnTypeNamespaceSyntax.ToString();

            this.Syntax.Meta.TryGetValue("GroupName", out var groupName);
            this.GroupName = groupName ?? DefaultGroupName;
        }

        public string CalcFullReturnType()
        {
            if (ReturnType == "bool")
                return "bool";

            return $"{ReturnNamespace}.{ReturnType}";
        }

        public override string ToString()
        {
            return this.FullName;
        }
    }
}
