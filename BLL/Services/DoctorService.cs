using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class DoctorService(IRepository<Doctor> repository, IMapper mapper):IDoctorService
{
    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto)
    {
        var doctor = mapper.Map<Doctor>(dto);

        await repository.CreateAsync(doctor);

        return mapper.Map<DoctorDto>(doctor);
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await repository.GetAllAsync();
        
        return doctors.Select(d => mapper.Map<DoctorDto>(d));
    }
}