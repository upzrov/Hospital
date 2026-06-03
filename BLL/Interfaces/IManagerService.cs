using BLL.DTOs.Manager;
namespace BLL.Interfaces;

public interface IManagerService
{
    Task<ManagerDto> CreateManager(CreateManagerDto dto);
    Task<IEnumerable<ManagerDto>> GetAllManagersAsync();
}