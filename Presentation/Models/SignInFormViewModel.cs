using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class SignInFormViewModel
{
    [Required]
    [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).+$")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
