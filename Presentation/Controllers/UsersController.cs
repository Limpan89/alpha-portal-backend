using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsersAsync();
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form")]
        public async Task<IActionResult> AddUser(AddUserFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.AddUserAsync(form.MapTo<AddUserFormDto>());
                return result.Succeeded ? Created() : BadRequest();
            }
            return
                Unauthorized();
        }

        [HttpPut]
        [Authorize]
        [Consumes("multipart/form")]
        public async Task<IActionResult> UpdateProject(EditUserFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(form.MapTo<EditUserFormDto>());
                return result.Succeeded ? Ok() : BadRequest();
            }
            return
                Unauthorized();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
