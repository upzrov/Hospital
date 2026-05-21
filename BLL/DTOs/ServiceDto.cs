namespace BLL.DTOs;

public class ServiceDto
{
    public int ServiceId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
}