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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllPateints()
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
    }
}
