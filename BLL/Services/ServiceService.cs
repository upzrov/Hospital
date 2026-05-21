using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class ServiceService(IRepository<Service> repository, IMapper mapper) : IServiceService
{
    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto)
    {
        if (!Enum.IsDefined(dto.Specialty))
        {
            throw new ArgumentException("Invalid specialty");
        }
        
        var service = mapper.Map<Service>(dto);
        
        await repository.CreateAsync(service);

        return mapper.Map<ServiceDto>(service);
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        var services = await repository.GetAllAsync();

        return services.Select(s => mapper.Map<ServiceDto>(s));
    }
}