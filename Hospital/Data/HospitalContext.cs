using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Data
{
    public class HospitalContext(DbContextOptions<HospitalContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}
