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
        [Authorize(Roles = "Manager")]
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
        
        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetServiceById([FromRoute] int serviceId)
        {
            return Ok(await serviceService.GetServiceByIdAsync(serviceId));
        }

        [HttpDelete("{serviceId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteService([FromRoute] int serviceId)
        {
            await serviceService.DeleteServiceAsync(serviceId);
            return NoContent();
        }
    }
}
