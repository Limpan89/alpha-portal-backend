using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions.Attributes;
using Swashbuckle.AspNetCore.Annotations;

// 90ish% Swagger doc ---AI - ChatGPT---

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ClientsController(IClientService clientService) : ControllerBase
    {
        private readonly IClientService _clientService = clientService;

        /// <summary>
        /// Retrieves all clients.
        /// </summary>
        /// <returns>A list of clients.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all clients")]
        [ProducesResponseType(typeof(IEnumerable<ClientModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Adds a new client. Requires admin API key.
        /// </summary>
        /// <param name="form">Form data to create a new client.</param>
        [HttpPost]
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Create a new client (admin only)")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Updates an existing client. Requires admin API key.
        /// </summary>
        /// <param name="form">Form data to update the client.</param>
        [HttpPut]
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Update an existing client (admin only)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Deletes a client by ID. Requires admin API key.
        /// </summary>
        /// <param name="id">The ID of the client to delete.</param>
        [HttpDelete("{id}")]
        [UseAdminApiKey]
        [SwaggerOperation(Summary = "Delete a client by ID (admin only)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
