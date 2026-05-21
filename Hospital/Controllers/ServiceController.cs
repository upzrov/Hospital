using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServiceController(IServiceService serviceService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto model)
        {
            try
            {
                var service = await serviceService.CreateServiceAsync(model);
                return Ok(service);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            return Ok(await serviceService.GetAllServicesAsync());
        }
    }
}
