using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StoreSample.Commands.Products
{
    [Route("api/v{version:apiVersion}/products")]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMediator _mediator;

        public Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create createCommand)
        {
            await _mediator.Send(createCommand);
            return Created($"api/v1/categories/{createCommand.Id}", createCommand.Id);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(Update updateCommand)
        {
            await _mediator.Send(updateCommand);
            return NoContent();
        }

        [HttpPost]
        [Route("{id}/image")]
        public async Task<IActionResult> UploadImage(UploadImage uploadImage)
        {
            await _mediator.Send(uploadImage);
            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Remove(Remove removeCommand)
        {
            await _mediator.Send(removeCommand);
            return NoContent();
        }
    }
}
