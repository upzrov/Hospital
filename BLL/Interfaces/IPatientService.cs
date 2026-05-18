using BLL.DTOs;

namespace BLL.Interfaces;

public interface IPatientService
{
    Task<PatientDto> Create(CreatePatientDto dto);
}