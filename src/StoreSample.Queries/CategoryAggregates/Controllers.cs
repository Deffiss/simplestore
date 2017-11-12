using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace StoreSample.Queries.CategoryAggregates
{
    [Route("api/v{version:apiVersion}/categories/aggregates")]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMediator _mediator;

        public Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CategoryAggregate[]))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAll());
            HttpContext.Response.Headers.Add("X-Total-Count", result.Length.ToString());
            return Ok(result);
        }
    }
}
