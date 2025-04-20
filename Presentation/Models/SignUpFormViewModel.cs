using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class SignUpFormViewModel
{
    [Required]
    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [Required]
    [Compare(nameof(ConfirmPassword))]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).+$")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;

    [Range(typeof(bool), "true", "true")]
    public bool TermsAndConditions { get; set; }
}
