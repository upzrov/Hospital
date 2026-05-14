namespace Hospital.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public required string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public List<Appointment> Appointments { get; } = [];
    }
}
