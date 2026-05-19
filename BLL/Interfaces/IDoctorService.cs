using BLL.DTOs;

namespace BLL.Interfaces;

public interface IDoctorService
{
    Task<DoctorDto> CreateAsync(CreateDoctorDto dto);
    Task<IEnumerable<DoctorDto>> GetAllAsync();
}