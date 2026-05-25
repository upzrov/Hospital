using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class AppointmentService(IRepository<Appointment> appointmentRepository,
    UserManager<User> userManager,
    IRepository<Service> serviceRepository,
    IRepository<Doctor> doctorRepository,
    IMapper mapper) : IAppointmentService
{
    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, string userId)
    {
        var user = await userManager.Users
            .Include(u => u.PatientProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        var service = await serviceRepository.GetByIdAsync(dto.ServiceId);
        
        var doctor = await doctorRepository
            .Query()
            .Include(d => d.Services)
            .FirstOrDefaultAsync(d => d.DoctorId == dto.DoctorId);
        
        await ValidateAppointmentAsync(service, doctor, dto);
        
        var appointment = new Appointment
        {
            StartAt = dto.StartAt,
            EndAt = dto.StartAt.AddMinutes(service!.DurationMinutes),
            DoctorId = dto.DoctorId,
            PatientId = user.PatientProfile.PatientId,
            ServiceId = dto.ServiceId
        };

        await appointmentRepository.CreateAsync(appointment);
        
        return mapper.Map<AppointmentDto>(appointment);
    }

    private async Task ValidateAppointmentAsync(Service? service, Doctor? doctor, CreateAppointmentDto dto)
    {
        if (service == null)
        {
            throw new KeyNotFoundException("Service not found");
        }

        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found");
        }

        if (dto.StartAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Appointment start time must be in the future");
        }

        var newStart = dto.StartAt;
        var newEnd = dto.StartAt.AddMinutes(service.DurationMinutes);

        var doctorIsBusy = await appointmentRepository.Query()
            .AnyAsync(a => a.DoctorId == doctor.DoctorId
                           && newStart < a.EndAt &&
                           newEnd > a.StartAt);

        if (!doctor.Services.Any(d => d.ServiceId == service.ServiceId))
        {
            throw new InvalidOperationException("Doctor does not have this service");
        }
        
        if (doctorIsBusy)
        {
            throw new InvalidOperationException("Doctor is busy at this time");
        }
    }
}