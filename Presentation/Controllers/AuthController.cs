using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

// 90ish% Swagger doc ---AI - ChatGPT---

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Signs in a user and returns a JWT token and user info.
        /// </summary>
        /// <param name="form">The sign-in credentials (email and password).</param>
        /// <returns>JWT token and user details on success; 404 if credentials are invalid.</returns>
        /// <response code="200">Returns JWT and user details</response>
        /// <response code="400">Invalid request model</response>
        /// <response code="404">User not found or invalid credentials</response>
        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SignIn(SignInFormViewModel form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


                var result = await _authService.SignInAsync(form.MapTo<SignInFormDto>());
                return result.Succeeded
                ? Ok(new
                { 
                    token = result.Token,
                    isAdmin = result.IsAdmin,
                    apiKey = result.ApiKey,
                    user = result.User 
                }) 
                : NotFound();
        }

        /// <summary>
        /// Signs up a new user with the given credentials.
        /// </summary>
        /// <param name="form">The sign-up form data (name, email, password).</param>
        /// <returns>Status 201 Created on success, or 400 BadRequest if invalid.</returns>
        /// <response code="201">User successfully created</response>
        /// <response code="400">Validation failed</response>
        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SingUp(SignUpFormViewModel form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

                var result = await _authService.SignUpAsync(form.MapTo<SignUpFormDto>());
                return result.Succeeded ? Created() : BadRequest();
        }
    }
}
