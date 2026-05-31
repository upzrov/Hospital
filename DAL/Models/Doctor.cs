using DAL.Enums;

namespace DAL.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public required string FullName { get; set; }
        public required Specialty Specialty { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhotoUrl { get; set; }
        public Gender Gender { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<Appointment> Appointments { get; } = [];
        public ICollection<Service> Services { get; set; } = [];
    }
}
