using BLL.DTOs;

namespace BLL.Interfaces;

public interface IServiceService
{
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto);
}