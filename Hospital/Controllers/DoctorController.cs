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
        [Authorize(Roles="Administrator")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPut("{doctorId}/services/{serviceId}")]
        public async Task<IActionResult> AssignServiceToDoctor(int doctorId, int serviceId)
        {
            await doctorService.AssignServiceToDoctorAsync(doctorId, serviceId);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{doctorId}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] int doctorId)
        {
            await doctorService.DeleteDoctorAsync(doctorId);
            return NoContent();
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpPut("{doctorId}")]
        public async Task<IActionResult> UpdateDoctor([FromRoute] int doctorId, [FromBody] UpdateDoctorDto moel)
        {
            await doctorService.UpdateDoctorAsync(doctorId, moel);
            return NoContent();
        }
    }
}
