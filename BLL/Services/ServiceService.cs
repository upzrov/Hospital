using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Service;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync(ServiceFilterDto filter)
    {
        var services = await serviceRepository.GetAllAsync();

        services = ApplyFilter(services, filter);

        return services.Select(s => mapper.Map<ServiceDto>(s));
    }

    public async Task DeleteServiceAsync(int serviceId)
    {
        var service = await serviceRepository.GetByIdAsync(serviceId);
        
        ValidateDeleteService(service);

        await serviceRepository.DeleteAsync(service!);
    }

    public async Task<ServiceDetailsDto> GetServiceByIdAsync(int serviceId)
    {
        var service = await serviceRepository
            .Query()
            .Include(s => s.Doctors)
            .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        
        if (service == null)
        {
            throw new KeyNotFoundException("Service not found");
        }
        
        return mapper.Map<ServiceDetailsDto>(service);
    }

    private IEnumerable<Service> ApplyFilter(IEnumerable<Service> services, ServiceFilterDto filter)
    {
        if (filter.Search != null)
        { 
            services = services.Where(s => s.Name.ToLower()
                .Contains(filter.Search.Trim().ToLower())).ToList();
        }

        if (filter.Specialty != null)
        {
            services = services.Where(s => s.Specialty == filter.Specialty).ToList();
        }
        
        services = filter.OrderBy switch

        {
            "price_desc" => services.OrderByDescending(s => s.Price),
            "price_asc" => services.OrderBy(s => s.Price),
            "name_desc" => services.OrderByDescending(s => s.Name),
            "name_asc" => services.OrderBy(s => s.Name),
            _ => services
        };
        
        return services;
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