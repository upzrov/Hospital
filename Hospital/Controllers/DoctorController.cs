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
            try
            {
                var doctor = await doctorService.CreateAsync(model);
                return Ok(doctor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            return Ok(await doctorService.GetAllAsync());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{doctorId}/services/{serviceId}")]
        public async Task<IActionResult> AssignServiceToDoctor(int doctorId, int serviceId)
        {
            try
            {
                await doctorService.AssignServiceToDoctorAsync(doctorId, serviceId);
                return NoContent();
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
    }
}
