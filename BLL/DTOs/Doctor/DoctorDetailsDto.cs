using DAL.Enums;

namespace BLL.DTOs;

public class DoctorDetailsDto
{
    public int DoctorId { get; set; }
    public required string FullName { get; set; }
    public required Specialty Specialty { get; set; }
    public required string Email { get; set; }
    public string? PhotoUrl { get; set; }
    public Gender Gender { get; set; }
    public TimeOnly WorkStart { get; set; }
    public TimeOnly WorkEnd { get; set; }
    public IEnumerable<ServiceDto> Services { get; set; } = [];
}