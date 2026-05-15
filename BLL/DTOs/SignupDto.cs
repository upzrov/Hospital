namespace BLL.DTOs
{
    public class SignupDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
    }
}
