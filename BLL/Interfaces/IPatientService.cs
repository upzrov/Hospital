using BLL.DTOs;

namespace BLL.Interfaces;

public interface IPatientService
{
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<IEnumerable<PatientDto>> GetAllAsync();
    Task DeleteAsync(int patientId);
}