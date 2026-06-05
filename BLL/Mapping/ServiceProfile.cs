using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Service;
using DAL.Models;

namespace BLL.Mapping;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Service, ServiceDto>();
        
        CreateMap<CreateServiceDto, Service>();
        
        CreateMap<Service, ServiceDetailsDto>();
    }
}