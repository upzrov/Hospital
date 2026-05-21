using System.ComponentModel.DataAnnotations;
using DAL.Enums;

namespace BLL.DTOs;

public class CreateServiceDto
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
    [MaxLength(20, ErrorMessage = "Name must be at most 20 characters long")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters long")]
    [MaxLength(1000, ErrorMessage = "Description must be at most 1000 characters long")]
    public required string Description { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
    public decimal Price { get; set; }
    
    public Specialty Specialty { get; set; }
}