using AutoMapper;
using BLL.DTOs.Manager;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace BLL.Interfaces;

public interface IManagerService
{
    Task<ManagerDto> CreateManager(CreateManagerDto dto);
}