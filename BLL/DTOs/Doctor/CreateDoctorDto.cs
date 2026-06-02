using System.ComponentModel.DataAnnotations;
using DAL.Enums;

namespace BLL.DTOs;

public class CreateDoctorDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long")]
    [MaxLength(40, ErrorMessage = "Full name must be at most 20 characters long")]
    public required string FullName { get; set; }
    
    [Required(ErrorMessage = "Specialty is required")]
    public required Specialty Specialty { get; set; }
    [Required(ErrorMessage = "Work hours are required")]
    public TimeOnly WorkStart { get; set; }
    [Required(ErrorMessage = "Work hours are required")]
    public TimeOnly WorkEnd { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Gender is required")]
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
}