using AutoMapper;
using BLL.DTOs;
using DAL.Models;

namespace BLL.Mapping;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<CreatePatientDto, Patient>();

        CreateMap<Patient, PatientDto>();
    }
}