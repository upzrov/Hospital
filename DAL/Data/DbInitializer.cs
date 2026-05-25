using DAL.Enums;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = ["Administrator", "Manager", "Patient"];

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminEmail = "admin@hospital.com";
            var adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    Name = "Administrator",
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            // Seed sample services, doctors and patients 
            var context = serviceProvider.GetRequiredService<HospitalContext>();

            // Services to add
            var servicesToAdd = new[]
            {
                new Service { Name = "General Consultation", Description = "General doctor consultation", Price = 50m, Specialty = Specialty.Therapist, DurationMinutes = 30 },
                new Service { Name = "Cardiology Check", Description = "Heart specialist consultation", Price = 120m, Specialty = Specialty.Cardiologist, DurationMinutes = 45 },
                new Service { Name = "Dental Visit", Description = "Dental check and cleaning", Price = 80m, Specialty = Specialty.Dentist, DurationMinutes = 30 },
                new Service { Name = "Neurology Exam", Description = "Neurology specialist exam", Price = 150m, Specialty = Specialty.Neurologist, DurationMinutes = 60 }
            };

            foreach (var svc in servicesToAdd)
            {
                var ex = await context.Services.FirstOrDefaultAsync(s => s.Name == svc.Name);
                if (ex == null)
                    context.Services.Add(svc);
                else
                {
                    ex.Description = svc.Description;
                    ex.Price = svc.Price;
                    ex.DurationMinutes = svc.DurationMinutes;
                    ex.Specialty = svc.Specialty;
                }
            }
            await context.SaveChangesAsync();

            // Doctors
            if (!await context.Doctors.AnyAsync())
            {
                var general = await context.Services.FirstOrDefaultAsync(s => s.Name == "General Consultation");
                var cardio = await context.Services.FirstOrDefaultAsync(s => s.Name == "Cardiology Check");

                var doc1 = new Doctor
                {
                    FullName = "Dr. John Smith",
                    Specialty = Specialty.Therapist,
                    Email = "john.smith@hospital.com",
                    PhoneNumber = "+380501112233"
                };
                if (general != null) doc1.Services.Add(general);

                var doc2 = new Doctor
                {
                    FullName = "Dr. Maria Petrenko",
                    Specialty = Specialty.Cardiologist,
                    Email = "m.petrenko@hospital.com",
                    PhoneNumber = "+380671234567"
                };
                if (cardio != null) doc2.Services.Add(cardio);
                // additional doctors for more specialties
                var dental = await context.Services.FirstOrDefaultAsync(s => s.Name == "Dental Visit");
                var neuro = await context.Services.FirstOrDefaultAsync(s => s.Name == "Neurology Exam");

                var doc3 = new Doctor
                {
                    FullName = "Dr. Olga Koval",
                    Specialty = Specialty.Dentist,
                    Email = "o.koval@hospital.com",
                    PhoneNumber = "+380631112233"
                };
                if (dental != null) doc3.Services.Add(dental);

                var doc4 = new Doctor
                {
                    FullName = "Dr. Petro Bondar",
                    Specialty = Specialty.Neurologist,
                    Email = "p.bondar@hospital.com",
                    PhoneNumber = "+380981234567"
                };
                if (neuro != null) doc4.Services.Add(neuro);

                context.Doctors.AddRange(doc1, doc2, doc3, doc4);
                await context.SaveChangesAsync();
            }

            // Users (patients)
            async Task<User> EnsureUser(string email, string pwd, string fullName)
            {
                var u = await userManager.FindByEmailAsync(email);
                if (u != null) return u;
                u = new User
                {
                    Email = email, 
                    UserName = email, 
                    Name = fullName, 
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                
                var r = await userManager.CreateAsync(u, pwd);
                
                if (r.Succeeded)
                {
                    if (await roleManager.RoleExistsAsync("Patient"))
                        await userManager.AddToRoleAsync(u, "Patient");
                }
                return u;
            }

            var u1 = await EnsureUser("patient1@hospital.com", "Patient123!", "Ivan Test");
            var u2 = await EnsureUser("patient2@hospital.com", "Patient123!", "Olena Ivanova");

            if (!await context.Patients.AnyAsync(p => p.UserId == u1.Id))
            {
                context.Patients.Add(new Patient { UserId = u1.Id, FullName = u1.Name });
            }
            if (!await context.Patients.AnyAsync(p => p.UserId == u2.Id))
            {
                context.Patients.Add(new Patient { UserId = u2.Id, FullName = u2.Name });
            }
            await context.SaveChangesAsync();
        }
    }
}
