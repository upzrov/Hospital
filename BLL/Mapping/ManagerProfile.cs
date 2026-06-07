using AutoMapper;
using BLL.DTOs.Manager;
using DAL.Models;

namespace BLL.Mapping;

public class ManagerProfile : Profile
{
    public ManagerProfile()
    {
        CreateMap<Manager, ManagerDto>();
        CreateMap<CreateManagerDto, Manager>();
        CreateMap<UpdateManagerDto, Manager>();
    }
}