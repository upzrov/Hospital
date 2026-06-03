using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class PatientService(IRepository<Patient> repository, IMapper mapper): IPatientService
{
    public async Task<PatientDto> CreateAsync(CreatePatientDto dto) 
    {
        ValidateCreate(dto);
        
        var patient = mapper.Map<Patient>(dto);
        
        await repository.CreateAsync(patient);

        return mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        var patients = await repository.GetAllAsync();

        return patients.Select(p => mapper.Map<PatientDto>(p));
    }

    public async Task DeleteAsync(int patientId)
    {
        var patient = await repository
            .Query()
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.PatientId == patientId);
        
        ValidateDelete(patient);

        await repository.DeleteAsync(patient!);
    }

    public async Task UpdateAsync(int patientId, UpdatePatientDto dto)
    {
        var patient = await repository.GetByIdAsync(patientId);
        
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        mapper.Map(dto, patient);
        
        await repository.UpdateAsync(patient);
    }

    private void ValidateCreate(CreatePatientDto dto)
    {
        if (dto.DateOfBirth > DateTime.UtcNow)
        {
            throw new ArgumentException("Date of birth cannot be in the future");
        }
    }
    
    private void ValidateDelete(Patient? patient)
    {
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        if (patient.Appointments.Any(a => a.EndAt > DateTime.UtcNow))
        {
            throw new InvalidOperationException("Patient has appointments");
        }
    }
}