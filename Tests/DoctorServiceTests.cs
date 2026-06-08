using AutoMapper;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
using DAL.Enums;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace Tests;

public class DoctorServiceTests
{
    private readonly Mock<IRepository<Doctor>> _doctorRepoMock;
    private readonly Mock<IRepository<Service>> _serviceRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

    private readonly IDoctorService _doctorService;

    public DoctorServiceTests()
    {
        _doctorRepoMock = new Mock<IRepository<Doctor>>();
        _serviceRepoMock = new Mock<IRepository<Service>>();
        _mapperMock = new Mock<IMapper>();

        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStoreMock.Object, null!, null!, null!, null!);

        _doctorService = new DoctorService(
            _doctorRepoMock.Object,
            _serviceRepoMock.Object,
            _mapperMock.Object,
            _userManagerMock.Object,
            _roleManagerMock.Object
        );
    }
    
    // Create doctor
    [Fact]
    public async Task CreateAsync_InvalidSpecialtyEnum_ThrowsArgumentException()
    {
        var dto = new CreateDoctorDto
        {
            Email = "doc@hosp.com",
            FullName = "Dr. House",
            Specialty = (Specialty)999, 
            WorkStart = new TimeOnly(9, 0),
            WorkEnd = new TimeOnly(17, 0)
        };
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _doctorService.CreateAsync(dto));
        Assert.Equal("Invalid specialty", exception.Message);
    }
    
    [Fact]
    public async Task CreateAsync_IdentityCreateFails_ThrowsIdentityValidationException()
    {
        var dto = new CreateDoctorDto
        {
            Email = "doc@hosp.com",
            FullName = "Dr. House",
            Specialty = Specialty.Cardiologist,
            WorkStart = new TimeOnly(9, 0),
            WorkEnd = new TimeOnly(17, 0),
            Password = "weak"
        };

        var identityErrors = new[] { new IdentityError { Code = "PasswordTooShort", Description = "Too short" } };
        var failedResult = IdentityResult.Failed(identityErrors);

        _userManagerMock
            .Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(failedResult);

        await Assert.ThrowsAsync<IdentityValidationException>(() => _doctorService.CreateAsync(dto));
    }
    
    // Assign service 
    [Fact]
    public async Task AssignServiceToDoctorAsync_SpecialtyMismatch_ThrowsInvalidOperationException()
    {
        var doctor = TestData.CreateDoctor(1);
        doctor.Specialty = Specialty.Cardiologist;
        var service = TestData.CreateService(1);
        service.Specialty = Specialty.Neurologist;

        var doctorMockQuery = new List<Doctor> { doctor }.BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorMockQuery);
        _serviceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(service);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _doctorService.AssignServiceToDoctorAsync(1, 1));
        
        Assert.Equal("Doctor and service must have the same specialty", exception.Message);
    }
    
    [Fact]
    public async Task AssignServiceToDoctorAsync_ServiceAlreadyAssigned_ThrowsInvalidOperationException()
    {
        var service = TestData.CreateService(1);
        var doctor = TestData.CreateDoctor(1);
        doctor.Services.Add(service);

        var doctorMockQuery = new List<Doctor> { doctor }.BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorMockQuery);
        _serviceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(service);
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _doctorService.AssignServiceToDoctorAsync(1, 1));
        
        Assert.Equal("Doctor already has this service", exception.Message);
    }
    
    // Delete doctor
    [Fact]
    public async Task DeleteDoctorAsync_DoctorHasActiveFutureAppointments_ThrowsInvalidOperationException()
    {
        var futureAppointment = new Appointment { AppointmentId = 10, EndAt = new DateTime(2030, 1, 1, 11, 0, 0) }; 
        var doctor = TestData.CreateDoctor(1);
        doctor.Appointments.Add(futureAppointment);

        var doctorMockQuery = new List<Doctor> { doctor }.BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorMockQuery);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _doctorService.DeleteDoctorAsync(1));
        Assert.Equal("Doctor has appointments", exception.Message);
        _doctorRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Doctor>()), Times.Never);
    }
    
    // Update doctor
    [Fact]
    public async Task UpdateDoctorAsync_NewHoursConflictWithExistingAppointments_ThrowsInvalidOperationException()
    {
        var appointment = new Appointment 
        { 
            StartAt = new DateTime(2030, 1, 1, 10, 0, 0), 
            EndAt = new DateTime(2030, 1, 1, 11, 0, 0)
        };
        var doctor = TestData.CreateDoctor(1);
        doctor.Appointments.Add(appointment);

        var updateDto = new UpdateDoctorDto
        {
            FullName = doctor.FullName,
            WorkStart = new TimeOnly(11, 0),
            WorkEnd = new TimeOnly(17, 0) 
        };

        var doctorMockQuery = new List<Doctor> { doctor }.BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorMockQuery);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _doctorService.UpdateDoctorAsync(1, updateDto));
        
        Assert.Equal("Doctor has appointments during this time", exception.Message);
    }
    
    // Delete service from the doctor
    [Fact]
    public async Task DeleteServiceFromDoctorAsync_HasActiveAppointmentsWithThisService_ThrowsInvalidOperationException()
    {
        var service = TestData.CreateService(1);
        
        var futureAppointment = new Appointment { ServiceId = 1, EndAt = new DateTime(2030, 1, 1, 11, 0, 0) }; 
        
        var doctor = TestData.CreateDoctor(1);
        doctor.Services.Add(service);
        doctor.Appointments.Add(futureAppointment);

        var doctorMockQuery = new List<Doctor> { doctor }.BuildMock();
        _doctorRepoMock.Setup(r => r.Query()).Returns(doctorMockQuery);
        _serviceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(service);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _doctorService.DeleteServiceFromDoctorAsync(1, 1));
        
        Assert.Equal("Doctor has appointments with this service", exception.Message);
    }
}