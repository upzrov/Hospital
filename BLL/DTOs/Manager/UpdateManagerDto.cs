using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Manager;

public class UpdateManagerDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long")]
    [MaxLength(40, ErrorMessage = "Full name must be at most 20 characters long")]
    public required string FullName { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }
}