using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]

    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("signin")]
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

        [HttpPost("signup")]
        public async Task<IActionResult> SingUp(SignUpFormViewModel form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

                var result = await _authService.SignUpAsync(form.MapTo<SignUpFormDto>());
                return result.Succeeded ? Created() : BadRequest();
        }
    }
}
