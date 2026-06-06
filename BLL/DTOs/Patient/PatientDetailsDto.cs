using BLL.DTOs.Appointment;

namespace BLL.DTOs;

public class PatientDetailsDto
{
    public int PatientId { get; set; }
    public required string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public IEnumerable<AppointmentDto>? Appointments { get; set; }
}