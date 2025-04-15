using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController(IClientService clientService) : ControllerBase
    {
        private readonly IClientService _clientService = clientService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clientService.GetClientsAsync();
            if (result.Succeeded)
                return Ok(result.Result);
            return
                Unauthorized(new { Errror = "Failed to get Clients" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _clientService.GetClientByIdAsync(id);
            if (result.Succeeded)
                return Ok(result.Result);
            return
                Unauthorized(new { Errror = "Failed to get Client" });
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(AddClientFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _clientService.AddClientAsync(form.MapTo<AddClientFormDto>());
                if (result.Succeeded)
                    return Ok(result);
            }
            return
                Unauthorized(new { Errror = "Failed to add Client" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProject(EditClientFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _clientService.UpdateClientAsync(form.MapTo<EditClientFormDto>());
                if (result.Succeeded)
                    return Ok(result);
            }
            return
                Unauthorized(new { Errror = "Failed to update Client" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            if (result.Succeeded)
                return Ok(result);
            return
                Unauthorized(new { Errror = "Failed to remove Client" });
        }
    }
}
