using MediatR.AspNetCore.CodeGenerator.CLI.Analyzers.Objects;
using System.Collections.Generic;
using System.Text;

namespace MediatR.AspNetCore.CodeGenerator.CLI
{
    class Renderer
    {
        public string RenderController(string ns, string controllerName, List<Request> requests)
        {
            var sb = new StringBuilder();

            sb.AppendLine($@"using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace {ns}
{{
    [Route(""api/[controller]/[Action]"")]
    [ApiController]
    public class {controllerName} : ControllerBase
    {{
        private readonly IMediator mediator;

        public {controllerName}(IMediator mediator)
        {{
            this.mediator = mediator;
        }}
");

            foreach (var item in requests)
                RenderMethod(sb, item);

            sb.AppendLine($@"    }}
}}
");

            return sb.ToString();
        }

        private void RenderMethod(StringBuilder sb, Request item)
        {
            sb.AppendLine($@"{item.Trivia}/// <param name=""command"">An instance of the {item.FullName}</param>
        /// <returns>The returned result of this command</returns>
        /// <response code=""201"">Returns the newly created item</response>
        /// <response code=""400"">If the item is null</response>   
        [HttpPost]
        [Produces(""application/json"")]
        [ProducesResponseType(typeof({item.FullReturnType}), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<{item.FullReturnType}> {item.Name}([FromBody] {item.FullName} command)
        {{
            return await this.mediator.Send(command);
        }}");
        }
    }
}
