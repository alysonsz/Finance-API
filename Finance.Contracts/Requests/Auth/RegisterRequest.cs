namespace Finance.Contracts.Requests.Auth;

public class RegisterRequest
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
