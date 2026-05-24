using AutoMapper;
using BLL.DTOs.Appointment;
using DAL.Models;

namespace BLL.Mapping;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        CreateMap<Appointment, AppointmentDto>();
    }
}