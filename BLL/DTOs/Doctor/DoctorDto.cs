using BLL.DTOs.Appointment;
using DAL.Enums;

namespace BLL.DTOs;

public class DoctorDto
{
    public int DoctorId { get; set; }
    public required string FullName { get; set; }
    public required Specialty Specialty { get; set; }
    public required string Email { get; set; }
    public string? PhotoUrl { get; set; }
    public Gender Gender { get; set; }
    public IEnumerable<ServiceDto> Services { get; set; } = [];
    public IEnumerable<AppointmentDto> Appointments { get; set; } = [];
}