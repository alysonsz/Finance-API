namespace Finance.Api;

public static class ApiConfiguration
{
    public static string ConnectionReadDatabase { get; set; } = string.Empty;
    public static string ConnectionWriteDatabase { get; set; } = string.Empty;
    public const string CorsPolicyName = "wasm";
}