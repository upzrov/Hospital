using DAL.Enums;

namespace BLL.DTOs;

public class CreateDoctorDto
{
    public required string FullName { get; set; }
    public required Specialty Specialty { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
}