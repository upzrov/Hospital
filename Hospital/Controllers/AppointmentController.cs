using System.Security.Claims;
using BLL.DTOs.Appointment;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var appointmentDto = await appointmentService.CreateAsync(model, userId!);
            return Ok(appointmentDto);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> GetAppointments()
        {
            return Ok(await appointmentService.GetAllAppointmentsAsync());
        }

        [HttpDelete("{appointmentId}")]
        [Authorize(Roles = "Patient, Manager")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] int appointmentId)
        {
            await appointmentService.DeleteAppointmentAsync(appointmentId);
            return NoContent();
        }

        [HttpGet("patient/appointments")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientAppointments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var appointments = await appointmentService.GetAppointmentsByPatientIdAsync(userId);
            return Ok(appointments);
        }
        
        [HttpGet("doctor/appointments")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var appointments = await appointmentService.GetAppointmentsByDoctorIdAsync(userId);
            return Ok(appointments);
        }

        [HttpGet("available-slots")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int doctorId, 
            [FromQuery] int serviceId, [FromQuery] DateTime date)
        {
            return Ok(await appointmentService.GetAvailableSlotsAsync(doctorId, serviceId, date));
        }
    }
}
