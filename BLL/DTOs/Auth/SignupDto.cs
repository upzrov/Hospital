using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class SignupDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }
        
        public required string Password { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [MaxLength(20, ErrorMessage = "Name must be at most 20 characters long")]
        public required string Name { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        [MinLength(3, ErrorMessage = "Last name must be at least 3 characters long")]
        [MaxLength(40, ErrorMessage = "Last name must be at most 20 characters long")]
        public required string LastName { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number")]
        public required string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime DateOfBirth { get; set; }
    }
}
