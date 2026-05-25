using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class DoctorService(IRepository<Doctor> doctorRepository, IRepository<Service> serviceRepository,
    IMapper mapper):IDoctorService
{
    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto)
    {
        if (!Enum.IsDefined(dto.Specialty))
        {
            throw new ArgumentException("Invalid specialty");
        }
        
        var doctor = mapper.Map<Doctor>(dto);

        await doctorRepository.CreateAsync(doctor);

        return mapper.Map<DoctorDto>(doctor);
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await doctorRepository
            .Query()
            .Include(d => d.Services)
            .Include(d => d.Appointments)
            .ToListAsync();
        
        return doctors.Select(d => mapper.Map<DoctorDto>(d));
    }

    public async Task AssignServiceToDoctorAsync(int doctorId, int serviceId)
    {
        var doctor = await doctorRepository.Query()
            .Include(d => d.Services)
            .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        
        var service = await serviceRepository.GetByIdAsync(serviceId);

        ValidateAssignService(doctor, service);
        
        doctor!.Services.Add(service!);
        await doctorRepository.UpdateAsync(doctor);
    }

    public async Task DeleteDoctorAsync(int doctorId)
    {
        var doctor = await doctorRepository
            .Query()
            .Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        
        ValidateDeleteDoctor(doctor);

        await doctorRepository.DeleteAsync(doctor!);
    }

    private void ValidateDeleteDoctor(Doctor? doctor)
    {
        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found");
        }

        if (doctor.Appointments.Any(a => a.EndAt > DateTime.UtcNow))
        {
            throw new InvalidOperationException("Doctor has appointments");
        }
    }

    private void ValidateAssignService(Doctor? doctor, Service? service)
    {
        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found");
        }

        if (service == null)
        {
            throw new KeyNotFoundException("Service not found");
        }

        if (doctor.Specialty != service.Specialty)
        {
            throw new InvalidOperationException("Doctor and service must have the same specialty");
        }

        if (doctor.Services.Any(d => d.ServiceId == service.ServiceId))
        {
            throw new InvalidOperationException("Doctor already has this service");
        }
    }
}