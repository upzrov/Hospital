using BLL.DTOs.Exception;

namespace BLL.DTOs
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public ErrorResponse? Error { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public string? Id { get; set; }
        public IList<string>? Roles { get; set; }
        public PatientDto? Patient { get; set; }
    }
}
