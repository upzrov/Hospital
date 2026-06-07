using AutoMapper;
using BLL.DTOs;
using DAL.Models;

namespace BLL.Mapping;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<CreatePatientDto, Patient>()
            .ForMember(dest => dest.FullName,
                opt => opt
                    .MapFrom(src => $"{src.Name} {src.LastName}"));

        CreateMap<Patient, PatientDto>();
        
        CreateMap<UpdatePatientDto, Patient>();
        
        CreateMap<Patient, PatientDetailsDto>();
    }
}