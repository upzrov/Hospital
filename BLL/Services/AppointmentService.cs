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
    IRepository<Patient> patientRepository,
    IMapper mapper) : IAppointmentService
{
    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, string userId)
    {
        var user = await userManager.Users
            .Include(u => u.PatientProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        
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
            PatientId = user.PatientProfile!.PatientId,
            ServiceId = dto.ServiceId
        };

        await appointmentRepository.CreateAsync(appointment);
        
        return mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
    {
        var appointments = await appointmentRepository.GetAllAsync();
        
        return appointments
            .OrderByDescending(a => a.StartAt)
            .Select(a => mapper.Map<AppointmentDto>(a));
    }

    public async Task DeleteAppointmentAsync(int appointmentId)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId);

        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        await appointmentRepository.DeleteAsync(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientIdAsync(string? userId)
    {
        var patient = patientRepository
            .Query()
            .FirstOrDefault(p => p.UserId == userId);

        if (patient == null)
        {
            throw new KeyNotFoundException("Patient not found");
        }

        var appointments = await appointmentRepository
            .Query()
            .Where(a => a.PatientId == patient.PatientId)
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();
        
        return appointments.Select(a => mapper.Map<AppointmentDto>(a));   
    }

    public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorIdAsync(string? userId)
    {
        var doctor = await doctorRepository
            .Query()
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found");
        }
        
        var appointments = await appointmentRepository
            .Query()
            .Where(a => a.DoctorId == doctor.DoctorId)
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();
        
        return appointments.Select(a => mapper.Map<AppointmentDto>(a));  
    }

    public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, int serviceId, DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return Enumerable.Empty<AvailableSlotDto>();
        }
        
        var doctor = await doctorRepository
            .Query()
            .Include(d => d.Services)
            .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

        if (doctor == null)
        {
            throw new KeyNotFoundException("Doctor not found");
        }

        if (!doctor.Services.Any(s => s.ServiceId == serviceId))
        {
            throw new KeyNotFoundException("Doctor does not have this service");
        }
        
        var service = await serviceRepository.GetByIdAsync(serviceId);
        
        DateTime startWork = date.Date + doctor.WorkStart.ToTimeSpan();
        DateTime endWork = date.Date + doctor.WorkEnd.ToTimeSpan();
        
        var slots = GenerateSlots(service!, startWork, endWork, date);
        
        var doctorAppointments = await appointmentRepository
            .Query()
            .Where(a => a.DoctorId == doctorId && a.StartAt.Date == date.Date)
            .ToListAsync();

        var availableSlots = slots
            .Where(s => s.StartAt > DateTime.Now)
            .Where(s => !doctorAppointments.Any(a => a.StartAt < s.EndAt && a.EndAt > s.StartAt))
            .ToList();

        return availableSlots;
    }

    private IEnumerable<AvailableSlotDto> GenerateSlots(
        Service service, DateTime startWork, DateTime endWork, DateTime date)
    {
        var slots = new List<AvailableSlotDto>();

        for (DateTime i = startWork;
             i.AddMinutes(service.DurationMinutes) <= endWork;
             i += TimeSpan.FromMinutes(service.DurationMinutes))
        {
            slots.Add(new AvailableSlotDto
            {
                StartAt = date.Date + i.TimeOfDay,
                EndAt = date.Date + i.AddMinutes(service.DurationMinutes).TimeOfDay
            });
        }
        
        return slots;
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
        
        if (!doctor.Services.Any(d => d.ServiceId == service.ServiceId))
        {
            throw new InvalidOperationException("Doctor does not have this service");
        }

        var newStart = dto.StartAt;
        var newEnd = dto.StartAt.AddMinutes(service.DurationMinutes);
        
        var workStart = dto.StartAt.Date + doctor.WorkStart.ToTimeSpan();
        var workEnd = dto.StartAt.Date + doctor.WorkEnd.ToTimeSpan();

        if (newStart < workStart || newEnd > workEnd)
        {
            throw new InvalidOperationException("Appointment is outside of doctor's working hours");
        }
        
        if (dto.StartAt.DayOfWeek == DayOfWeek.Saturday || dto.StartAt.DayOfWeek == DayOfWeek.Sunday)
        {
            throw new InvalidOperationException("Doctor is not available on weekends");
        }
        
        var doctorIsBusy = await appointmentRepository.Query()
            .AnyAsync(a => a.DoctorId == doctor.DoctorId
                           && newStart < a.EndAt &&
                           newEnd > a.StartAt);
        
        if (doctorIsBusy)
        {
            throw new InvalidOperationException("Doctor is busy at this time");
        }
    }
}