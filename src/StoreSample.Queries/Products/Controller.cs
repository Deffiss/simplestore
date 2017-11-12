using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace StoreSample.Queries.Products
{
    [Route("api/v{version:apiVersion}/products")]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMediator _mediator;

        public Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Product))]
        public async Task<IActionResult> GetById(GetById query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [HttpGet("categories/{categoryid}/products")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Product[]))]
        public async Task<IActionResult> GetAll(GetAll query)
        {
            var result = await _mediator.Send(query);
            HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            return Ok(result.Products);
        }
    }
}
