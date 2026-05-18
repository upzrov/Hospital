using BLL.DTOs;

namespace BLL.Interfaces;

public interface IDoctorService
{
    Task<DoctorDto> Create(CreateDoctorDto dto);
}