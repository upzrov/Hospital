using BLL.DTOs.Appointment;

namespace BLL.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, string userId);
}