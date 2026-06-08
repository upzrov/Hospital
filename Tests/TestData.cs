using DAL.Models;

namespace Tests;

public static class TestData
{
    public static User CreateUser(string userId)
    {
        return new User
        {
            Id = userId,
            Name = "Test User",
            PatientProfile = new Patient
            {
                PatientId = 1,
                FullName = "Test User"
            }
        };
    }

    public static Patient CreatePatient(int patientId)
    {
        return new Patient
        {
            PatientId = patientId,
            FullName = "Test Patient",
            UserId = "1"
        };
    }

    public static Service CreateService(int serviceId)
    {
        return new Service
        {
            ServiceId = serviceId,
            Name = "Test Service",
            Price = 100,
            DurationMinutes = 60,
            Specialty = 0,
            Description = "Test Description"
        };
    }

    public static Doctor CreateDoctor(int doctorId)
    {
        return new Doctor
        {
            UserId = "1",
            DoctorId = doctorId,
            WorkStart = new TimeOnly(8, 0),
            WorkEnd = new TimeOnly(17, 0),
            FullName = "Test Doctor",
            Specialty = 0,
            Email = "",
            PhotoUrl = "https://randomuser.me/api/portraits/men/1.jpg"
        };
    }

    public static Appointment CreateAppointment(int appointmentId)
    {
        return new Appointment
        {
            AppointmentId = appointmentId,
            DoctorId = 1,
            ServiceId = 1,
            StartAt = new DateTime(2030, 06, 11, 10, 0, 0),
            EndAt = new DateTime(2030, 06, 11, 11, 0, 0)
        };
    }
}