using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class PatientService(IRepository<Patient> repository, IMapper mapper): IPatientService
{
    public async Task<PatientDto> CreateAsync(CreatePatientDto dto) 
    {
        var patient = mapper.Map<Patient>(dto);
        
        await repository.CreateAsync(patient);

        return mapper.Map<PatientDto>(patient);
    }
}