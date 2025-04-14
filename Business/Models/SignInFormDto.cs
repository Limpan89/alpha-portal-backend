using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Business.Models;

public class SignInFormDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
