using AutoMapper;
using BLL.DTOs;
using DAL.Models;

namespace BLL.Mapping;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Service, ServiceDto>();
        
        CreateMap<CreateServiceDto, Service>();
    }
}