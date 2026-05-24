namespace DAL.Models
{
    public class Appointment
    {
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }

        public int DoctorId { get; set; }
        public  Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }

}
