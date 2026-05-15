namespace DAL.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public required string FullName { get; set; }
        public required string Specialty { get; set; }
        public string? ContactInfo { get; set; }

        public List<Appointment> Appointments { get; } = [];
    }
}
