using DAL.Enums;

namespace BLL.DTOs.Service;

public class ServiceDetailsDto
{
    public int ServiceId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public Specialty Specialty { get; set; }
    public IEnumerable<DoctorDto> Doctors { get; set; } = [];
}