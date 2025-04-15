using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInFormViewModel form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (ModelState.IsValid)
            {
                var result = await _authService.SignInAsync(form.MapTo<SignInFormDto>());
                if (result.Succeeded)
                    return Ok(new { token = result.Result });
            }
            return Unauthorized(new { error = "Invalid email or password." });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SingUp(SignUpFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.SignUpAsync(form.MapTo<SignUpFormDto>());
                if (result.Succeeded)
                    return Ok(new { message = "User Created" });
                return Unauthorized(new { error = "Invalid signup form.", status = result.StatusCode });
            }
            return Unauthorized(new { error = "Invalid signup form." });
        }
    }
}
