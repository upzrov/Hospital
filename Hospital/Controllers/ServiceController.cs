using BLL.DTOs;
using BLL.DTOs.Service;
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
            var service = await serviceService.CreateServiceAsync(model);
            return Ok(service);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices([FromQuery] ServiceFilterDto filter)
        {
            return Ok(await serviceService.GetAllServicesAsync(filter));
        }

        [HttpDelete("{serviceId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteService([FromRoute] int serviceId)
        {
            await serviceService.DeleteServiceAsync(serviceId);
            return NoContent();
        }
    }
}
