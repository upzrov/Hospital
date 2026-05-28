namespace BLL.DTOs.Exception;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;

    public Dictionary<string, string[]>? Errors { get; set; }

    public int StatusCode { get; set; }
}