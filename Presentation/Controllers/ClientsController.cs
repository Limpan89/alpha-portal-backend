using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ClientsController(IClientService clientService) : ControllerBase
    {
        private readonly IClientService _clientService = clientService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clientService.GetAllClientsAsync();
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _clientService.GetClientByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpPost]
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
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
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
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
        [UseAdminApiKey]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
