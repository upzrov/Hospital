using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class PatientService(IRepository<Patient> repository): IPatientService
{
    public async Task<PatientDto> CreateAsync(CreatePatientDto dto) 
    {
        var patient = new Patient
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            UserId = dto.UserId
        };
        
        await repository.CreateAsync(patient);

        return new PatientDto
        {
            PatientId = patient.PatientId,
            FullName = patient.FullName,
            PhoneNumber = patient.PhoneNumber,
            DateOfBirth = patient.DateOfBirth,
            UserId = patient.UserId
        };
    }
}