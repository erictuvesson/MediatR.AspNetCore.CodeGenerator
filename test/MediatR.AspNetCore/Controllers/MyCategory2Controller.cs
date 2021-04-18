using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediatR.AspNetCore.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class MyCategory2Controller : ControllerBase
    {
        private readonly IMediator mediator;

        public MyCategory2Controller(IMediator mediator)
        {
            this.mediator = mediator;
        }


        /// <summary>
        /// My Summary
        /// </summary>
        /// <param name="command">An instance of the MediatR.TestData.MyRequest2</param>
        /// <returns>The returned result of this command</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>   
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<bool> MyRequest2([FromBody] MediatR.TestData.MyRequest2 command)
        {
            return await this.mediator.Send(command);
        }
    }
}

