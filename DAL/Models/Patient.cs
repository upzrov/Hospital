using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public required string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }
        public List<Appointment> Appointments { get; } = [];
    }
}
