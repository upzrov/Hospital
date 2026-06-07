using BLL.DTOs;

namespace BLL.Interfaces;

public interface IDoctorService
{
    Task<DoctorDto> CreateAsync(CreateDoctorDto dto);
    Task<IEnumerable<DoctorDto>> GetAllAsync();
    Task AssignServiceToDoctorAsync(int doctorId, int serviceId);
    Task DeleteDoctorAsync(int doctorId);
    Task UpdateDoctorAsync(int doctorId, UpdateDoctorDto dto);
    Task<DoctorDetailsDto> GetDoctorByIdAsync(int doctorId);
    Task<DoctorDetailsDto> GetDoctorByUserIdAsync(string userId);
    Task DeleteServiceFromDoctorAsync(int doctorId, int serviceId);
}