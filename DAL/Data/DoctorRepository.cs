using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class DoctorRepository(HospitalContext context) 
    : Repository<Doctor>(context)
{ 
    public override async Task<Doctor?> GetByIdAsync(int id)
    {
        return await context.Doctors
            .Include(d => d.Services)
            .FirstOrDefaultAsync(d => d.DoctorId == id);
    }
}