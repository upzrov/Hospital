using AutoMapper;
using BLL.DTOs.Manager;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services;

public class ManagerService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, 
    IMapper mapper, IRepository<Manager> managerRepository) : IManagerService
{
    public async Task<ManagerDto> CreateManager(CreateManagerDto dto)
    {
        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            Name = dto.FullName,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        
        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            throw new IdentityValidationException(result.Errors);
        }
        
        if (await roleManager.RoleExistsAsync("Manager"))
        {
            await userManager.AddToRoleAsync(user, "Manager");
        }

        var manager = mapper.Map<Manager>(dto);
        
        manager.User = user;

        await managerRepository.CreateAsync(manager);

        return mapper.Map<ManagerDto>(manager);
    }

    public async Task<IEnumerable<ManagerDto>> GetAllManagersAsync()
    {
        var managers = await managerRepository.GetAllAsync();

        return managers.Select(m => mapper.Map<ManagerDto>(m));
    }

    public async Task UpdateManagerAsync(int managerId, UpdateManagerDto dto)
    {
        var manager = await managerRepository.GetByIdAsync(managerId);

        if (manager == null)
        {
            throw new KeyNotFoundException("Manager not found");
        }
        
        mapper.Map(dto, manager);
        await managerRepository.UpdateAsync(manager);
    }

    public async Task DeleteManagerAsync(int managerId)
    {
        var manager = await managerRepository.GetByIdAsync(managerId);
        
        if (manager == null)
        {
            throw new KeyNotFoundException("Manager not found");
        }
        
        await managerRepository.DeleteAsync(manager);
    }
}