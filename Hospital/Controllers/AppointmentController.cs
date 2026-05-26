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
            try
            {
                var appointmentDto = await appointmentService.CreateAsync(model, userId!);
                return Ok(appointmentDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAppointments()
        {
            return Ok(await appointmentService.GetAllAppointmentsAsync());
        }

        [HttpDelete("{appointmentId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] int appointmentId)
        {
            try
            {
                await appointmentService.DeleteAppointmentAsync(appointmentId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
