using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class ServiceService(IRepository<Service> serviceRepository, 
    IRepository<Appointment> appointmentRepository,
    IMapper mapper) : IServiceService
{
    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto)
    {
        if (!Enum.IsDefined(dto.Specialty))
        {
            throw new ArgumentException("Invalid specialty");
        }
        
        var service = mapper.Map<Service>(dto);
        
        await serviceRepository.CreateAsync(service);

        return mapper.Map<ServiceDto>(service);
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        var services = await serviceRepository.GetAllAsync();

        return services.Select(s => mapper.Map<ServiceDto>(s));
    }

    public async Task DeleteServiceAsync(int serviceId)
    {
        var service = await serviceRepository.GetByIdAsync(serviceId);
        
        ValidateDeleteService(service);

        await serviceRepository.DeleteAsync(service!);
    }

    private void ValidateDeleteService(Service? service)
    {
        if (service == null)
        {
            throw new KeyNotFoundException("Service not found");
        }

        if (appointmentRepository.Query().Any(a =>
                a.ServiceId == service.ServiceId && a.EndAt > DateTime.UtcNow))
        {
            throw new InvalidOperationException("Cannot delete service with active appointments");
        }
    }
}