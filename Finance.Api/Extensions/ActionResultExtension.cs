using Finance.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Extensions
{
    public static class ActionResultExtensions
    {
        public static IActionResult FromResponse<T>(this ControllerBase controller, Response<T> response)
        {
            return response.IsSuccess
                ? controller.Ok(response)
                : controller.BadRequest(new
                {
                    status = response.IsSuccess ? 200 : response._code,
                    message = response.Message,
                    data = response.Data
                });
        }
    }
}
