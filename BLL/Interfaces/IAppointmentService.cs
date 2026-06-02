using BLL.DTOs.Appointment;

namespace BLL.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, string userId);
    Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
    Task DeleteAppointmentAsync(int appointmentId);
    Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientIdAsync(string? userId);
    Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorIdAsync(string? userId);
    Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, int serviceId, DateTime date);
}