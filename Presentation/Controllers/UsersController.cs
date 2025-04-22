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
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all users")]
        [ProducesResponseType(typeof(IEnumerable<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsersAsync();
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        /// <summary>
        /// Retrieves a specific user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a user by ID")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound();
        }

        /// <summary>
        /// Adds a new user. Admin access required.
        /// </summary>
        /// <param name="form">The user creation form.</param>
        [HttpPost]
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Add a new user (admin only)")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Updates a user. Admin access required.
        /// </summary>
        /// <param name="form">The update user form.</param>
        [HttpPut]
        [UseAdminApiKey]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Update an existing user (admin only)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Deletes a user by ID. Admin access required.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        [HttpDelete("{id}")]
        [UseAdminApiKey]
        [SwaggerOperation(Summary = "Delete a user by ID (admin only)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveProject(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
