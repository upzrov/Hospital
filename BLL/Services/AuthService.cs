using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using BLL.DTOs;
using BLL.DTOs.Exception;
using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    public class AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration, IPatientService patientService) : IAuthService
    {
        public async Task<AuthResult> SignUpAsync(SignupDto model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new AuthResult { IsSuccess = false, Error = new ErrorResponse
                {
                    Message = "User already exists.",
                    StatusCode = (int) HttpStatusCode.BadRequest
                }
                };
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    IsSuccess = false, Error = new ErrorResponse
                    {
                        Message = "Validation failed",
                        Errors = new Dictionary<string, string[]>
                            { { "password", result.Errors.Select(e => e.Description).ToArray() } },
                        StatusCode = (int) HttpStatusCode.BadRequest
                    }
                };
            }

            var patient = await patientService.CreateAsync(new CreatePatientDto
            {
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                UserId = user.Id
            });
            
            if (await roleManager.RoleExistsAsync("Patient"))
            {
                await userManager.AddToRoleAsync(user, "Patient");
            }

            return await GenerateAuthResultAsync(user, patient);
        }

        public async Task<AuthResult> SignInAsync(SigninDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResult { IsSuccess = false, Error = new ErrorResponse
                {
                    Message = "Invalid password or email",
                    StatusCode = (int) HttpStatusCode.BadRequest
                } };
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return new AuthResult { IsSuccess = false, Error = new ErrorResponse
                {
                    Message = "Invalid password or email",
                    StatusCode = (int) HttpStatusCode.BadRequest
                } };
            }

            return await GenerateAuthResultAsync(user);
        }

        private async Task<AuthResult> GenerateAuthResultAsync(User user, PatientDto? patient = null)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return new AuthResult
            {
                IsSuccess = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Id = user.Id,
                Roles = userRoles,
                Patient = patient
            };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            return new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}
