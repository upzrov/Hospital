using AutoMapper;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class PatientService(IRepository<Patient> patientRepository, IMapper mapper, UserManager<User> userManager): IPatientService
{
    public async Task<PatientDto> CreateAsync(CreatePatientDto dto) 
    {
        ValidateCreate(dto);
        
        var patient = mapper.Map<Patient>(dto);
        
        await patientRepository.CreateAsync(patient);

        return mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        var patients = await patientRepository.GetAllAsync();

        return patients.Select(p => mapper.Map<PatientDto>(p));
    }

    public async Task DeleteAsync(int patientId)
    {
        var patient = await patientRepository
            .Query()
            .Include(p => p.Appointments)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.PatientId == patientId);
        
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        if (patient.Appointments.Any(a => a.EndAt > DateTime.UtcNow))
        {
            throw new InvalidOperationException("Patient has appointments");
        }

        if (patient.User == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var result = await userManager.DeleteAsync(patient.User);

        if (!result.Succeeded)
        {
            throw new IdentityValidationException(result.Errors);
        }

        await patientRepository.DeleteAsync(patient);
    }

    public async Task UpdateAsync(int patientId, UpdatePatientDto dto)
    {
        var patient = await patientRepository.GetByIdAsync(patientId);
        
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        mapper.Map(dto, patient);
        
        await patientRepository.UpdateAsync(patient);
    }

    public async Task<PatientDto> GetPatientByIdAsync(string userId)
    {
        var patient = await patientRepository
            .Query()
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();
        
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }
        
        return mapper.Map<PatientDto>(patient);
    }

    public async Task<PatientDetailsDto> GetPatientByIdAsync(int patientId)
    {
        var patient = await patientRepository
            .Query()
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.PatientId == patientId);
        
        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }
        
        return mapper.Map<PatientDetailsDto>(patient);   
    }

    private void ValidateCreate(CreatePatientDto dto)
    {
        if (dto.DateOfBirth > DateTime.UtcNow)
        {
            throw new ArgumentException("Date of birth cannot be in the future");
        }
    }
}