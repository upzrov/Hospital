namespace DAL.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        
        public int DoctorId { get; set; }
        public  Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }

}
