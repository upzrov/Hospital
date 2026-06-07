using BLL.DTOs.Manager;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagerController(IManagerService managerService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateManager([FromBody] CreateManagerDto model)
        {
            var manager = await managerService.CreateManager(model);
            return Ok(manager);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> GetAllManagers()
        {
            var managers = await managerService.GetAllManagersAsync();
            return Ok(managers);
        }
        
        [HttpPut("{managerId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateManager([FromRoute] int managerId, [FromBody] UpdateManagerDto model)
        {
            await managerService.UpdateManagerAsync(managerId, model);
            return NoContent();
        }

        [HttpDelete("{managerId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteManager([FromRoute] int managerId)
        {
            await managerService.DeleteManagerAsync(managerId);
            return NoContent();
        }
    }
}
