using System.Text.Json.Serialization;

namespace Finance.Domain.Responses;

public class Response<TData>
{
    private int _code = 200;

    [JsonConstructor]
    public Response()
        => _code = 200;

    public Response(TData? data, int code = 200, string? message = null)
    {
        Data = data;
        _code = code;
        Message = message;
    }

    public TData? Data { get; set; }
    public string? Message { get; set; }

    [JsonIgnore]
    public bool IsSuccess => _code >= 200 && _code <= 299;
}