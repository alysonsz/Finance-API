namespace Finance.Contracts.Requests.Auth;

public class UpdateUserProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
