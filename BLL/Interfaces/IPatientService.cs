using BLL.DTOs;

namespace BLL.Interfaces;

public interface IPatientService
{
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
}