using BLL.DTOs;
using BLL.DTOs.Service;

namespace BLL.Interfaces;

public interface IServiceService
{
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto);
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync(ServiceFilterDto filter);
    Task DeleteServiceAsync(int serviceId);
}