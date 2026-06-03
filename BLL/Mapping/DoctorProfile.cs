using AutoMapper;
using BLL.DTOs;
using DAL.Models;

namespace BLL.Mapping;

public class DoctorProfile : Profile
{
    public DoctorProfile()
    {
        CreateMap<CreateDoctorDto, Doctor>();

        CreateMap<Doctor, DoctorDto>();
        
        CreateMap<UpdateDoctorDto, Doctor>();
    }
}