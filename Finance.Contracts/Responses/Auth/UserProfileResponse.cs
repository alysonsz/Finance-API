namespace Finance.Contracts.Responses.Auth;

public class UserProfileResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
