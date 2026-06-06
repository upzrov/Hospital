using BLL.DTOs;

namespace BLL.Interfaces;

public interface IPatientService
{
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<IEnumerable<PatientDto>> GetAllAsync();
    Task DeleteAsync(int patientId);
    Task UpdateAsync(int patientId, UpdatePatientDto dto);
    Task<PatientDto> GetPatientByIdAsync(string userId);
    Task<PatientDetailsDto> GetPatientByIdAsync(int patientId);
}