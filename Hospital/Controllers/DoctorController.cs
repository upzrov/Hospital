using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController(IDoctorService doctorService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles="Administrator")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto model)
        {
            var doctor = await doctorService.Create(model);
            
            return Ok(doctor);
        }
    }
}
