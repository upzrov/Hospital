using Microsoft.AspNetCore.Identity;

namespace DAL.Models
{
    public class User : IdentityUser
    {
        public required string Name { get; set; }

        public Patient? PatientProfile { get; set; }
    }
}
