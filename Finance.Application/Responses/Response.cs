using System.Text.Json.Serialization;

namespace Finance.Application.Responses;

public class Response
{
    public int _code = 200;

    public string? Message { get; set; }

    [JsonIgnore]
    public bool IsSuccess => _code >= 200 && _code <= 299;

    public Response(string? message, int code)
    {
        Message = message;
        _code = code;
    }

    public Response() { }

    public static Response Success(string? message = "Operação realizada com sucesso.")
        => new(message, 200);

    public static Response Fail(string? message)
        => new(message, 400);
}

public class Response<TData> : Response
{
    public TData? Data { get; set; }
    public Response(TData? data, int code = 200, string? message = null) : base(message, code)
    {
        Data = data;
    }

    [JsonConstructor]
    public Response() : base() { }

    public static Response<TData> Success(TData? data, string? message = "Operação realizada com sucesso.")
        => new(data, 200, message);

    public static Response<TData> Fail(string? message)
        => new(default, 400, message);
}