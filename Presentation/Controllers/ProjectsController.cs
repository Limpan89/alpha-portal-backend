using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        private readonly IProjectService _projectService = projectService;
        /// <summary>
        /// Retrieves all projects.
        /// </summary>
        /// <returns>A list of projects.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all projects")]
        [ProducesResponseType(typeof(IEnumerable<ProjectModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _projectService.GetAllProjectsAsync();
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        /// <summary>
        /// Retrieves a project by ID.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>The requested project data.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a project by ID")]
        [ProducesResponseType(typeof(ProjectModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        /// <summary>
        /// Creates a new project. Requires admin API key.
        /// </summary>
        /// <param name="form">Form data for the new project.</param>
        [HttpPost]
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Create a new project (admin only)")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddProject(AddProjectFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _projectService.AddProjectAsync(form.MapTo<AddProjectFormDto>());
                return result.Succeeded ? Created() : BadRequest();
            }
            return
                Unauthorized();
        }

        /// <summary>
        /// Updates an existing project. Requires admin API key.
        /// </summary>
        /// <param name="form">Updated form data for the project.</param>
        [HttpPut]
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Update an existing project (admin only)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProject(EditProjectFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _projectService.UpdateProjectAsync(form.MapTo<EditProjectFormDto>());
                return result.Succeeded ? Ok() : BadRequest();
            }
            return
                Unauthorized();
        }

        /// <summary>
        /// Deletes a project by ID. Requires admin API key.
        /// </summary>
        /// <param name="id">The ID of the project to delete.</param>
        [HttpDelete("{id}")]
        [UseAdminApiKey]
        [SwaggerOperation(Summary = "Delete a project by ID (admin only)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
