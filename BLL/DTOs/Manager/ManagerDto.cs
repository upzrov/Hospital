namespace BLL.DTOs.Manager;

public class ManagerDto
{
    public int ManagerId { get; set; }
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}