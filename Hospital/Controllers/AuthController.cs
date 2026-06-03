using System.Security.Claims;
using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignupDto model)
        {
            var result = await authService.SignUpAsync(model);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            SetTokenCookie(result.Token!, result.Expiration!.Value);

            return Ok(result.Patient);
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SigninDto model)
        {
            var result = await authService.SignInAsync(model);

            if (!result.IsSuccess)
            {
                return Unauthorized(result.Error);
            }

            SetTokenCookie(result.Token!, result.Expiration!.Value);

            return Ok();
        }

        [HttpPost("sign-out")]
        public new IActionResult SignOut()
        {
            Response.Cookies.Delete("token");
            return Ok();
        }

        [Authorize]
        [HttpGet("role")]
        public IActionResult GetRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(role);
        }

        private void SetTokenCookie(string token, DateTime expiration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiration
            };

            Response.Cookies.Append("token", token, cookieOptions);
        }
    }
}