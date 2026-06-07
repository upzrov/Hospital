using System.Security.Claims;
using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatientController(IPatientService patientService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> GetAllPatients()
        {
            return Ok(await patientService.GetAllAsync());
        }
        
        [HttpDelete("{patientId}")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DeletePatient([FromRoute] int patientId)
        {
            await patientService.DeleteAsync(patientId);
            return NoContent();
        }

        [HttpPut("{patientId}")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> UpdatePatient([FromRoute] int patientId, [FromBody] UpdatePatientDto model)
        {
            await patientService.UpdateAsync(patientId, model);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientById()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var patient = await patientService.GetPatientByIdAsync(userId);
            
            return Ok(patient);
        }

        [HttpGet("{patientId}")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> GetPatientById([FromRoute] int patientId)
        {
            var patient = await patientService.GetPatientByIdAsync(patientId);
            return Ok(patient);
        }
    }
}
