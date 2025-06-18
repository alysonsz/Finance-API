namespace Finance.Application.Requests.Auth;

public class LoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
