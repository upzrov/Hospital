using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class DoctorService(IRepository<Doctor> repository):IDoctorService
{
    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto)
    {
        var doctor = new Doctor
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Specialty = dto.Specialty
        };

        await repository.CreateAsync(doctor);

        return new DoctorDto
        {
            DoctorId = doctor.DoctorId,
            FullName = doctor.FullName,
            Email = doctor.Email,
            PhoneNumber = doctor.PhoneNumber,
            Specialty = doctor.Specialty
        };
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await repository.GetAllAsync();
        
        return doctors.Select(d => new DoctorDto
        {
            DoctorId = d.DoctorId,
            FullName = d.FullName,
            Email = d.Email,
            PhoneNumber = d.PhoneNumber,
            Specialty = d.Specialty
        });
    }
}