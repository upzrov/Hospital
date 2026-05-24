namespace BLL.DTOs.Appointment;

public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? Status { get; set; }

    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int ServiceId { get; set; }
}