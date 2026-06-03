using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class HospitalContext(DbContextOptions<HospitalContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Manager> Managers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(u => u.PatientProfile)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId);

            builder.Entity<Doctor>()
                .Property(d => d.Specialty)
                .HasConversion<string>();
            
            builder.Entity<Service>()
                .Property(d => d.Specialty)
                .HasConversion<string>();
        }
    }
}
