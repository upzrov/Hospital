using AutoMapper;
using BLL.DTOs.Appointment;
using DAL.Models;

namespace BLL.Mapping;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.StartAt, 
                opt => opt.MapFrom(src => src.StartAt.ToLocalTime()))
            .ForMember(dest => dest.EndAt, 
                opt => opt.MapFrom(src => src.EndAt.ToLocalTime()));
    }
}