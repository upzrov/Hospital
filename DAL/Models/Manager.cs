using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Manager
{
    public int ManagerId { get; set; }
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    [ForeignKey("User")]
    public string? UserId { get; set; }
    public User? User { get; set; }
}