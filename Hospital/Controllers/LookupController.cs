using BLL.Extensions;
using Microsoft.AspNetCore.Mvc;
using DAL.Enums;

namespace PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        [HttpGet("specialties")]
        public IActionResult GetSpecialties()
        {
            return Ok(Enum.GetValues<Specialty>()
                .Select(s => new
                {
                    Id = (int)s,
                    Name = s.ToDisplayName()
                }).ToList());
        }
        
        [HttpGet("genders")]
        public IActionResult GetGenders()
        {
            return Ok(Enum.GetValues<Gender>()
                .Select(s => new
                {
                    Id = (int)s,
                    Name = s.ToDisplayName()
                }).ToList());
        }
    }
}
