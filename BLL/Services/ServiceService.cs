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
        var service = mapper.Map<Service>(dto);
        
        await repository.CreateAsync(service);

        return mapper.Map<ServiceDto>(service);
    }
}