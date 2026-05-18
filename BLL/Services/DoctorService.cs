using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class DoctorService(IRepository<Doctor> repository):IDoctorService
{
    public async Task<DoctorDto> Create(CreateDoctorDto dto)
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
}