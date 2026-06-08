using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using static MockQueryable.MockQueryableExtensions;
using Moq;

namespace Tests;

public class AppointmentServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IRepository<Appointment>> _appointmentRepoMock;
    private readonly Mock<IRepository<Service>> _serviceRepoMock;
    private readonly Mock<IRepository<Doctor>> _doctorRepoMock;
    private readonly Mock<IRepository<Patient>> _patientRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    
    private readonly IAppointmentService _appointmentService;

    public AppointmentServiceTests()
    {
        // Ініціалізуємо класичні Moq-обгортки
        _appointmentRepoMock = new Mock<IRepository<Appointment>>();
        _serviceRepoMock = new Mock<IRepository<Service>>();
        _doctorRepoMock = new Mock<IRepository<Doctor>>();
        _patientRepoMock = new Mock<IRepository<Patient>>();
        _mapperMock = new Mock<IMapper>();

        // Налаштовуємо UserManager, щоб компілятор не сварився на null!
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        // Збираємо наш сервіс
        _appointmentService = new AppointmentService(
            _appointmentRepoMock.Object,
            _userManagerMock.Object,
            _serviceRepoMock.Object,
            _doctorRepoMock.Object,
            _patientRepoMock.Object,
            _mapperMock.Object
        );
    }
    
    //Create appointment
    [Fact]
    public async Task CreateAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = DateTime.UtcNow.AddDays(1)
        };

        var emptyUsersMockQuery = new List<User>().BuildMock();

        _userManagerMock.Setup(u => u.Users).Returns(emptyUsersMockQuery);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.CreateAsync(dto, "non-existent-user-id"));

        Assert.Equal("User not found", exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_ServiceNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = DateTime.UtcNow.AddDays(1)
        };

        var user = TestData.CreateUser("1");
        var doctor = TestData.CreateDoctor(1);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor> { doctor }.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync((Service?)null);
        
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.CreateAsync(dto, "1"));
        
        Assert.Equal("Service not found", exception.Message);   
    }
    
    [Fact]
    public async Task CreateAsync_DoctorNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = DateTime.UtcNow.AddDays(1)
        };

        var user = TestData.CreateUser("1");
        
        var service = TestData.CreateService(1);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor>{}.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync(service);
        
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.CreateAsync(dto, "1"));
        
        Assert.Equal("Doctor not found", exception.Message); 
    }
    
    [Fact]
    public async Task CreateAsync_AppointmentStartTimeIsBeforeNow_ThrowsArgumentException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = DateTime.UtcNow.AddDays(-1)
        };

        var user = TestData.CreateUser("1");
        var service = TestData.CreateService(1);
        var doctor = TestData.CreateDoctor(1);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor>{ doctor }.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync(service);
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _appointmentService.CreateAsync(dto, "1"));
        
        Assert.Equal("Appointment start time must be in the future", exception.Message); 
    }
    
    [Fact]
    public async Task CreateAsync_DoctorDoesNotHaveService_ThrowsInvalidOperationException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = new DateTime(2026, 06, 11, 10,0,0)
        };

        var user = TestData.CreateUser("1");
        var service = TestData.CreateService(1);
        var doctor = TestData.CreateDoctor(1);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor>{ doctor }.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync(service);
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _appointmentService.CreateAsync(dto, "1"));
        
        Assert.Equal("Doctor does not have this service", exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_AppointmentIsOutsideDoctorWorkHours_ThrowsInvalidOperationException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = new DateTime(2026, 06, 11, 17,0,0)
        };

        var user = TestData.CreateUser("1");
        var service = TestData.CreateService(1);
        var doctor = TestData.CreateDoctor(1);
        doctor.WorkEnd = new TimeOnly(16, 0, 0);
        doctor.Services.Add(service);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor>{ doctor }.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync(service);
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _appointmentService.CreateAsync(dto, "1"));
        
        Assert.Equal("Appointment is outside of doctor's working hours", exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_DoctorIsBusy_ThrowsInvalidOperationException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = new DateTime(2026, 06, 11, 10,0,0)
        };

        var appointment = TestData.CreateAppointment(1);

        var user = TestData.CreateUser("1");
        var service = TestData.CreateService(1);
        var doctor = TestData.CreateDoctor(1);
        doctor.Services.Add(service);
        doctor.Appointments.Add(appointment);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor>{ doctor }.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        var appointmentMockQuery = new List<Appointment> { appointment }.BuildMock();
        _appointmentRepoMock.Setup(a => a.Query()).Returns(appointmentMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync(service);
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _appointmentService.CreateAsync(dto, "1"));
        
        Assert.Equal("Doctor is busy at this time", exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_AppointmentIsOnWeekend_ThrowsInvalidOperationException()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = new DateTime(2026, 06, 13, 10,0,0)
        };

        var user = TestData.CreateUser("1");
        var service = TestData.CreateService(1);
        var doctor = TestData.CreateDoctor(1);
        doctor.Services.Add(service);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor>{ doctor }.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync(service);
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _appointmentService.CreateAsync(dto, "1"));
        
        Assert.Equal("Doctor is not available on weekends", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_AppointmentIsCreatedSuccessfully()
    {
        var dto = new CreateAppointmentDto
        {
            DoctorId = 1,
            ServiceId = 1,
            StartAt = new DateTime(2026, 06, 11, 10,0,0)
        };

        var user = TestData.CreateUser("1");
        var service = TestData.CreateService(1);
        var doctor = TestData.CreateDoctor(1);
        doctor.Services.Add(service);
        
        var userMockQuery = new List<User> { user }.BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(userMockQuery);
        
        var doctorMockQuery = new List<Doctor>{ doctor }.BuildMock();
        _doctorRepoMock.Setup(d => d.Query()).Returns(doctorMockQuery);
        
        _serviceRepoMock.Setup(s => s.GetByIdAsync(dto.ServiceId)).ReturnsAsync(service);
        
        var appointmentMockQuery = new List<Appointment>().BuildMock(); 
        _appointmentRepoMock.Setup(r => r.Query()).Returns(appointmentMockQuery);
        
        var fakeDto = new AppointmentDto 
        { 
            AppointmentId = 1, 
            DoctorId = dto.DoctorId, 
            StartAt = dto.StartAt 
        };
        
        _mapperMock
            .Setup(m => m.Map<AppointmentDto>(It.IsAny<Appointment>()))
            .Returns(fakeDto);
        
        var result = await _appointmentService.CreateAsync(dto, "1");

        Assert.NotNull(result);
        
        _appointmentRepoMock.Verify(r => r.CreateAsync(It.IsAny<Appointment>()), Times.Once);
    }
    
    //Get all appointments
    [Fact] 
    public async Task GetAllAppointmentsAsync_ReturnsAllAppointments()
    {
        var appointment1 = new Appointment { AppointmentId = 1, StartAt = new DateTime(2026, 06, 11, 10, 0, 0) };
        var appointment2 = new Appointment { AppointmentId = 2, StartAt = new DateTime(2026, 06, 11, 15, 0, 0) }; // Найновіший
        var appointment3 = new Appointment { AppointmentId = 3, StartAt = new DateTime(2026, 06, 11, 09, 0, 0) }; // Найстаріший

        var fakeAppointmentsList = new List<Appointment> { appointment1, appointment2, appointment3 };

        _appointmentRepoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(fakeAppointmentsList);
        
        _mapperMock
            .Setup(m => m.Map<AppointmentDto>(It.IsAny<Appointment>()))
            .Returns((Appointment source) => new AppointmentDto 
            { 
                AppointmentId = source.AppointmentId, 
                StartAt = source.StartAt 
            });

        var result = (await _appointmentService.GetAllAppointmentsAsync()).ToList();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count); 
        
        Assert.Equal(2, result[0].AppointmentId);
    
        Assert.Equal(1, result[1].AppointmentId);
    
        Assert.Equal(3, result[2].AppointmentId);
    }
    
    //Delete appointment
    [Fact]
    public async Task DeleteAsync_AppointmentNotFound_ThrowsKeyNotFoundException()
    {
        int nonExistentId = 999;
        
        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Appointment?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.DeleteAppointmentAsync(nonExistentId));

        Assert.Equal("Appointment not found", exception.Message);
    }
    
    [Fact]
    public async Task DeleteAsync_AppointmentDeletedSuccessfully()
    {
        int existingId = 1;
        var fakeAppointment = TestData.CreateAppointment(existingId);

        _appointmentRepoMock
            .Setup(r => r.GetByIdAsync(existingId))
            .ReturnsAsync(fakeAppointment);

        _appointmentRepoMock
            .Setup(r => r.DeleteAsync(fakeAppointment))
            .Returns(Task.CompletedTask);

        await _appointmentService.DeleteAppointmentAsync(existingId);

        _appointmentRepoMock.Verify(r => r.DeleteAsync(fakeAppointment), Times.Once);
    }
    
    [Fact]
    public async Task GetAppointmentsByPatientIdAsync_PatientNotFound_ThrowsKeyNotFoundException()
    {
        var patientsMockQuery = new List<Patient>().BuildMock();
        _patientRepoMock.Setup(r => r.Query()).Returns(patientsMockQuery);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.GetAppointmentsByPatientIdAsync("1"));

        Assert.Equal("Patient not found", exception.Message);
    }

    [Fact]
    public async Task GetAppointmentsByPatientIdAsync_ReturnsAppointmentsOrderedDescending()
    {
        var fakePatient = TestData.CreatePatient(1);
    
        var appointmentOld = new Appointment { AppointmentId = 1, PatientId = 1, StartAt = new DateTime(2026, 06, 11, 10, 0, 0) };
        var appointmentNew = new Appointment { AppointmentId = 2, PatientId = 1, StartAt = new DateTime(2026, 06, 11, 14, 0, 0) }; 
        var otherAppointment = new Appointment { AppointmentId = 3, PatientId = 999, StartAt = new DateTime(2026, 06, 11, 12, 0, 0) }; 

        var patientsMockQuery = new List<Patient> { fakePatient }.BuildMock();
        var appointmentsMockQuery = new List<Appointment> { appointmentOld, appointmentNew, otherAppointment }.BuildMock();

        _patientRepoMock.Setup(r => r.Query()).Returns(patientsMockQuery);
        _appointmentRepoMock.Setup(r => r.Query()).Returns(appointmentsMockQuery);

        _mapperMock
            .Setup(m => m.Map<AppointmentDto>(It.IsAny<Appointment>()))
            .Returns((Appointment source) => new AppointmentDto 
            { 
                AppointmentId = source.AppointmentId, 
                StartAt = source.StartAt 
            });

        var result = (await _appointmentService.GetAppointmentsByPatientIdAsync("1")).ToList();

        Assert.NotNull(result);
    
        Assert.Equal(2, result.Count); 

        Assert.Equal(2, result[0].AppointmentId); 
        Assert.Equal(1, result[1].AppointmentId); 
    }

    [Fact]
    public async Task GetAppointmentsByDoctorIdAsync_DoctorNotFound_ThrowsKeyNotFoundException()
    {
        var doctorsMockQuery = new List<Doctor>().BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorsMockQuery);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.GetAppointmentsByDoctorIdAsync("1"));

        Assert.Equal("Doctor not found", exception.Message);
    }

    [Fact]
    public async Task GetAppointmentsByDoctorIdAsync_ReturnsAppointmentsOrderedDescending()
    {
        var fakeDoctor = TestData.CreateDoctor(1);
    
        var appointmentMorning = new Appointment { AppointmentId = 10, DoctorId = 1, StartAt = new DateTime(2026, 06, 11, 09, 0, 0) };
        var appointmentEvening = new Appointment { AppointmentId = 11, DoctorId = 1, StartAt = new DateTime(2026, 06, 11, 18, 0, 0) }; 
        var otherDoctorAppointment = new Appointment { AppointmentId = 12, DoctorId = 999, StartAt = new DateTime(2026, 06, 11, 12, 0, 0) }; 

        var doctorsMockQuery = new List<Doctor> { fakeDoctor }.BuildMock();
        var appointmentsMockQuery = new List<Appointment> { appointmentMorning, appointmentEvening, otherDoctorAppointment }.BuildMock();

        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorsMockQuery);
        _appointmentRepoMock.Setup(r => r.Query()).Returns(appointmentsMockQuery);

        _mapperMock
            .Setup(m => m.Map<AppointmentDto>(It.IsAny<Appointment>()))
            .Returns((Appointment source) => new AppointmentDto 
            { 
                AppointmentId = source.AppointmentId, 
                StartAt = source.StartAt 
            });

        var result = (await _appointmentService.GetAppointmentsByDoctorIdAsync("1")).ToList();

        Assert.NotNull(result);
    
        Assert.Equal(2, result.Count); 

        Assert.Equal(11, result[0].AppointmentId); 
        Assert.Equal(10, result[1].AppointmentId); 
    }
    
    //Get available slots
    [Fact]
    public async Task GetAvailableSlotsAsync_DateIsWeekend_ReturnsEmptyEnumerable()
    {
        DateTime weekendDate = new DateTime(2026, 06, 13);
        
        var result = await _appointmentService.GetAvailableSlotsAsync(1, 1, weekendDate);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetAvailableSlotsAsync_DoctorNotFound_ThrowsKeyNotFoundException()
    {
        DateTime weekday = new DateTime(2026, 06, 11); 

        var emptyDoctorsMockQuery = new List<Doctor>().BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(emptyDoctorsMockQuery);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.GetAvailableSlotsAsync(10, 1, weekday));

        Assert.Equal("Doctor not found", exception.Message);
    }
    
    [Fact]
    public async Task GetAvailableSlotsAsync_DoctorDoesNotHaveService_ThrowsKeyNotFoundException()
    {
        DateTime weekday = new DateTime(2026, 06, 11); 

        var doctor = TestData.CreateDoctor(1);

        var doctorsMockQuery = new List<Doctor> { doctor }.BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorsMockQuery);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appointmentService.GetAvailableSlotsAsync(1, 1, weekday));

        Assert.Equal("Doctor does not have this service", exception.Message);
    }
    
    [Fact]
    public async Task GetAvailableSlotsAsync_DoctorExists_ReturnsOnlyAvailableSlots()
    {
        DateTime date = new DateTime(2026, 06, 11); 

        var service = TestData.CreateService(1);

        var doctor = TestData.CreateDoctor(1);
        doctor.WorkStart = new TimeOnly(9, 0);  
        doctor.WorkEnd = new TimeOnly(12, 0); 
        doctor.Services.Add(service);

        var existingAppointment = new Appointment
        {
            AppointmentId = 5,
            DoctorId = 1,
            ServiceId = 1,
            StartAt = new DateTime(2026, 06, 11, 10, 0, 0),
            EndAt = new DateTime(2026, 06, 11, 11, 0, 0)
        };
        doctor.Appointments.Add(existingAppointment);

        var doctorMockQuery = new List<Doctor> { doctor }.BuildMock();
        var appointmentMockQuery = new List<Appointment> { existingAppointment }.BuildMock();

        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorMockQuery);
        _appointmentRepoMock.Setup(r => r.Query()).Returns(appointmentMockQuery);
        _serviceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(service);

        var result = (await _appointmentService.GetAvailableSlotsAsync(1, 1, date)).ToList();

        Assert.NotNull(result);
    
        Assert.Equal(2, result.Count);

        // Перший слот: 09:00 - 10:00
        Assert.Equal(new DateTime(2026, 06, 11, 9, 0, 0), result[0].StartAt);
    
        // Другий слот: 11:00 - 12:00 
        Assert.Equal(new DateTime(2026, 06, 11, 11, 0, 0), result[1].StartAt);
    }
}