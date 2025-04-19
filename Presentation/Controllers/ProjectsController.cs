using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions.Attributes;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        private readonly IProjectService _projectService = projectService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _projectService.GetAllProjectsAsync();
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpPost]
        [Authorize]
        [UseAdminApiKey]
        [Consumes("multipart/form")]
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

        [HttpPut]
        [Authorize]
        [UseAdminApiKey]
        [Consumes("multipart/form")]
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

        [HttpDelete("{id}")]
        [Authorize]
        [UseAdminApiKey]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
