namespace BLL.DTOs;

public class CreatePatientDto
{
    public required string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserId { get; set; }
}