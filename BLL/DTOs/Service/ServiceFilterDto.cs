using DAL.Enums;

namespace BLL.DTOs.Service;

public class ServiceFilterDto
{
    public string? Search { get; set; }
    public Specialty? Specialty { get; set; }
    public string? OrderBy { get; set; }
}