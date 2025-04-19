namespace Business.Models;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public int StatusCode { get; set; }
    public string? Token { get; set; }
    public bool IsAdmin { get; set; }
    public string? ApiKey { get; set; }
    public string? UserId { get; set; }
}
