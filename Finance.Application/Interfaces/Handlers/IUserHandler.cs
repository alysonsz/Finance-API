using Finance.Application.Requests.Auth;
using Finance.Application.Responses;

namespace Finance.Application.Interfaces.Handlers;

public interface IUserHandler
{
    Task<Response<string>> LoginAsync(LoginRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
}
