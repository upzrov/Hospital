namespace BLL.DTOs.Appointment;

public class CreateAppointmentDto
{
    public required DateTime StartAt { get; set; }

    public required int DoctorId { get; set; }
    public required int ServiceId { get; set; }
}