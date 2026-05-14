namespace Hospital.Models
{
    public enum UserRole
    {
        Administrator,
        Manager,
        RegisteredUser
    }

    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        // Optional link to a patient profile if the registered user is a patient
        public Patient? PatientProfile { get; set; }
    }
}
