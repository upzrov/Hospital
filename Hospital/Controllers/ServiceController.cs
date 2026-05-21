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
            var service = await serviceService.CreateServiceAsync(model);
            
            return Ok(service);
        }
    }
}
