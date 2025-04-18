using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ClientsController(IClientService clientService) : ControllerBase
    {
        private readonly IClientService _clientService = clientService;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clientService.GetClientsAsync();
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _clientService.GetClientByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form")]
        [SwaggerOperation(Summary = "Create new Client")]
        public async Task<IActionResult> AddProject(AddClientFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientAsync(form.MapTo<AddClientFormDto>());
                return result.Succeeded ? Created() : BadRequest();
            }
            return
                Unauthorized();
        }

        [HttpPut]
        [Authorize]
        [Consumes("multipart/form")]
        public async Task<IActionResult> UpdateProject(EditClientFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _clientService.UpdateClientAsync(form.MapTo<EditClientFormDto>());
                return result.Succeeded ? Ok() : BadRequest();
            }
            return
                Unauthorized();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
