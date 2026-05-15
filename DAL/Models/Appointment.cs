namespace DAL.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }

        public int DoctorId { get; set; }
        public required Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public required Patient Patient { get; set; }
    }

}
