using Finance.Contracts.Requests.Auth;
using Finance.Contracts.Responses;

namespace Finance.Contracts.Interfaces.Handlers;

public interface IUserHandler
{
    Task<Response<string>> LoginAsync(LoginRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
}
