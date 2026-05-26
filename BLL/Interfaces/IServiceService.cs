using BLL.DTOs;

namespace BLL.Interfaces;

public interface IServiceService
{
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto);
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
    Task DeleteServiceAsync(int serviceId);
}