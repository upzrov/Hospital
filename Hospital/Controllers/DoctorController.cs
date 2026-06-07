using System.Security.Claims;
using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DoctorController(IDoctorService doctorService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles="Manager")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto model)
        {
            var doctor = await doctorService.CreateAsync(model);
            return Ok(doctor);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            return Ok(await doctorService.GetAllAsync());
        }
        
        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetDoctorById([FromRoute] int doctorId)
        {
            return Ok(await doctorService.GetDoctorByIdAsync(doctorId));
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{doctorId}/services/{serviceId}")]
        public async Task<IActionResult> AssignServiceToDoctor(int doctorId, int serviceId)
        {
            await doctorService.AssignServiceToDoctorAsync(doctorId, serviceId);
            return NoContent();
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{doctorId}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] int doctorId)
        {
            await doctorService.DeleteDoctorAsync(doctorId);
            return NoContent();
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{doctorId}")]
        public async Task<IActionResult> UpdateDoctor([FromRoute] int doctorId, [FromBody] UpdateDoctorDto moel)
        {
            await doctorService.UpdateDoctorAsync(doctorId, moel);
            return NoContent();
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("me")]
        public async Task<IActionResult> GetDoctorById()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var doctor = await doctorService.GetDoctorByUserIdAsync(userId);
            return Ok(doctor);
        }
        
        [Authorize(Roles = "Manager")]
        [HttpDelete("{doctorId}/services/{serviceId}")]
        public async Task<IActionResult> DeleteServiceFromDoctor(int doctorId, int serviceId)
        {
            await doctorService.DeleteServiceFromDoctorAsync(doctorId, serviceId);
            return NoContent();
        }
    }
}
