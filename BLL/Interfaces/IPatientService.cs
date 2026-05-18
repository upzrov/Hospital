using BLL.DTOs;

namespace BLL.Services;

public interface IPatientService
{
    Task<PatientDto> Create(CreatePatientDto dto);
}