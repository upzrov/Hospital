using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class UpdateDoctorDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long")]
    [MaxLength(40, ErrorMessage = "Full name must be at most 20 characters long")]
    public required string FullName { get; set; }
    
    [Required(ErrorMessage = "Work hours are required")]
    public TimeOnly WorkStart { get; set; }
    [Required(ErrorMessage = "Work hours are required")]
    public TimeOnly WorkEnd { get; set; }
}