namespace Finance.Contracts.Requests.Auth;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
