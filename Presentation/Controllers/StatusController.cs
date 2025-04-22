using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// 90ish% Swagger doc ---AI - ChatGPT---

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class StatusController(IStatusService statusService) : ControllerBase
    {
        private readonly IStatusService _statusService = statusService;

        /// <summary>
        /// Retrieves all status entries.
        /// </summary>
        /// <returns>A list of statuses.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all statuses")]
        [ProducesResponseType(typeof(IEnumerable<StatusModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _statusService.GetAllStatusAsync();
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        /// <summary>
        /// Retrieves a specific status by ID.
        /// </summary>
        /// <param name="id">The ID of the status.</param>
        /// <returns>The requested status data.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a status by ID")]
        [ProducesResponseType(typeof(StatusModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _statusService.GetStatusByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }
    }
}
