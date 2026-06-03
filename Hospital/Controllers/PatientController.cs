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
        [Authorize(Roles = "Administrator")]
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
    }
}
