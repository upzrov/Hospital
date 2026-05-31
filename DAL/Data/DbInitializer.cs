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

            string[] roleNames = ["Administrator", "Manager", "Patient", "Doctor"];

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
                new Service { Name = "Консультація терапевта", Description = "Загальна консультація лікаря", Price = 50m, Specialty = Specialty.Therapist, DurationMinutes = 30 },
                new Service { Name = "Огляд терапевта з аналізами", Description = "Консультація та необхідні аналізи", Price = 100m, Specialty = Specialty.Therapist, DurationMinutes = 40 },
                new Service { Name = "Кардіологічний огляд", Description = "Консультація кардіолога", Price = 120m, Specialty = Specialty.Cardiologist, DurationMinutes = 45 },
                new Service { Name = "ЕКГ з консультацією", Description = "ЕКГ + консультація кардіолога", Price = 180m, Specialty = Specialty.Cardiologist, DurationMinutes = 60 },
                new Service { Name = "Стоматологічний огляд", Description = "Огляд та чистка зубів", Price = 80m, Specialty = Specialty.Dentist, DurationMinutes = 30 },
                new Service { Name = "Лікування каріесу", Description = "Лікування простого каріесу", Price = 150m, Specialty = Specialty.Dentist, DurationMinutes = 45 },
                new Service { Name = "Неврологічне обстеження", Description = "Консультація невролога", Price = 150m, Specialty = Specialty.Neurologist, DurationMinutes = 60 },
                new Service { Name = "Невролог з рефлексотерапією", Description = "Консультація невролога та рефлексотерапія", Price = 200m, Specialty = Specialty.Neurologist, DurationMinutes = 90 }
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
                var general1 = await context.Services.FirstOrDefaultAsync(s => s.Name == "Консультація терапевта");
                var general2 = await context.Services.FirstOrDefaultAsync(s => s.Name == "Огляд терапевта з аналізами");
                var cardio1 = await context.Services.FirstOrDefaultAsync(s => s.Name == "Кардіологічний огляд");
                var cardio2 = await context.Services.FirstOrDefaultAsync(s => s.Name == "ЕКГ з консультацією");
                var dental1 = await context.Services.FirstOrDefaultAsync(s => s.Name == "Стоматологічний огляд");
                var dental2 = await context.Services.FirstOrDefaultAsync(s => s.Name == "Лікування каріесу");
                var neuro1 = await context.Services.FirstOrDefaultAsync(s => s.Name == "Неврологічне обстеження");
                var neuro2 = await context.Services.FirstOrDefaultAsync(s => s.Name == "Невролог з рефлексотерапією");

                // Therapists
                var doc1 = new Doctor
                {
                    FullName = "Іван Петренко",
                    Specialty = Specialty.Therapist,
                    Email = "ivan@hospital.com",
                    PhoneNumber = "+380501112233",
                    PhotoUrl = "https://randomuser.me/api/portraits/men/32.jpg"
                };
                if (general1 != null) doc1.Services.Add(general1);
                if (general2 != null) doc1.Services.Add(general2);

                var doc2 = new Doctor
                {
                    FullName = "Анна Сидоренко",
                    Specialty = Specialty.Therapist,
                    Email = "anna.sidorenko@hospital.com",
                    PhoneNumber = "+380501234567",
                    PhotoUrl = "https://randomuser.me/api/portraits/women/72.jpg"
                };
                if (general1 != null) doc2.Services.Add(general1);

                // Cardiologists
                var doc3 = new Doctor
                {
                    FullName = "Марія Коваль",
                    Specialty = Specialty.Cardiologist,
                    Email = "m.koval@hospital.com",
                    PhoneNumber = "+380671234567",
                    PhotoUrl = "https://randomuser.me/api/portraits/women/22.jpg"
                };
                if (cardio1 != null) doc3.Services.Add(cardio1);
                if (cardio2 != null) doc3.Services.Add(cardio2);

                var doc4 = new Doctor
                {
                    FullName = "Сергій Мартинов",
                    Specialty = Specialty.Cardiologist,
                    Email = "s.martinov@hospital.com",
                    PhoneNumber = "+380671112233",
                    PhotoUrl = "https://randomuser.me/api/portraits/men/22.jpg"
                };
                if (cardio1 != null) doc4.Services.Add(cardio1);

                // Dentists
                var doc5 = new Doctor
                {
                    FullName = "Ольга Бондар",
                    Specialty = Specialty.Dentist,
                    Email = "o.bondar@hospital.com",
                    PhoneNumber = "+380631112233",
                    PhotoUrl = "https://randomuser.me/api/portraits/women/21.jpg"
                };
                if (dental1 != null) doc5.Services.Add(dental1);
                if (dental2 != null) doc5.Services.Add(dental2);

                var doc6 = new Doctor
                {
                    FullName = "Павло Захарченко",
                    Specialty = Specialty.Dentist,
                    Email = "p.zakharenko@hospital.com",
                    PhoneNumber = "+380639876543",
                    PhotoUrl = "https://randomuser.me/api/portraits/men/21.jpg"
                };
                if (dental1 != null) doc6.Services.Add(dental1);

                // Neurologists
                var doc7 = new Doctor
                {
                    FullName = "Петро Шевченко",
                    Specialty = Specialty.Neurologist,
                    Email = "p.shevchenko@hospital.com",
                    PhoneNumber = "+380981234567",
                    PhotoUrl = "https://randomuser.me/api/portraits/men/20.jpg"
                };
                if (neuro1 != null) doc7.Services.Add(neuro1);
                if (neuro2 != null) doc7.Services.Add(neuro2);

                var doc8 = new Doctor
                {
                    FullName = "Людмила Грищенко",
                    Specialty = Specialty.Neurologist,
                    Email = "l.grishchenko@hospital.com",
                    PhoneNumber = "+380989876543",
                    PhotoUrl = "https://randomuser.me/api/portraits/women/20.jpg"
                };
                if (neuro1 != null) doc8.Services.Add(neuro1);

                
                var d1 = await EnsureUser("doctor1@hospital.com", "Doctor123!", doc1.FullName, "Doctor");
                var d2 = await EnsureUser("doctor2@hospital.com", "Doctor123!", doc2.FullName, "Doctor");
                var d3 = await EnsureUser("doctor3@hospital.com", "Doctor123!", doc3.FullName, "Doctor");
                var d4 = await EnsureUser("doctor4@hospital.com", "Doctor123!", doc4.FullName, "Doctor");
                var d5 = await EnsureUser("doctor5@hospital.com", "Doctor123!", doc5.FullName, "Doctor");
                var d6 = await EnsureUser("doctor6@hospital.com", "Doctor123!", doc6.FullName, "Doctor");
                var d7 = await EnsureUser("doctor7@hospital.com", "Doctor123!", doc7.FullName, "Doctor");
                var d8 = await EnsureUser("doctor8@hospital.com", "Doctor123!", doc8.FullName, "Doctor");

                doc1.UserId = d1.Id;
                doc2.UserId = d2.Id;
                doc3.UserId = d3.Id;
                doc4.UserId = d4.Id;
                doc5.UserId = d5.Id;
                doc6.UserId = d6.Id;
                doc7.UserId = d7.Id;
                doc8.UserId = d8.Id;

                context.Doctors.AddRange(doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc8);
                await context.SaveChangesAsync();
            }

            // Users 
            async Task<User> EnsureUser(string email, string pwd, string fullName, string role)
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
                    if (await roleManager.RoleExistsAsync(role))
                        await userManager.AddToRoleAsync(u, role);
                }
                return u;
            }

            var p1 = await EnsureUser("patient1@hospital.com", "Patient123!", "Іван Іванов", "Patient");
            var p2 = await EnsureUser("patient2@hospital.com", "Patient123!", "Олена Іванова", "Patient");
            var p3 = await EnsureUser("patient3@hospital.com", "Patient123!", "Максим Петров", "Patient");
            var p4 = await EnsureUser("patient4@hospital.com", "Patient123!", "Тетяна Сидоренко", "Patient");
            var p5 = await EnsureUser("patient5@hospital.com", "Patient123!", "Дмитро Коваленко", "Patient");
            var p6 = await EnsureUser("patient6@hospital.com", "Patient123!", "Марія Грищенко", "Patient");
            var p7 = await EnsureUser("patient7@hospital.com", "Patient123!", "Олексій Зінкевич", "Patient");

            if (!await context.Patients.AnyAsync(p => p.UserId == p1.Id))
            {
                context.Patients.Add(new Patient { UserId = p1.Id, FullName = p1.Name });
            }
            if (!await context.Patients.AnyAsync(p => p.UserId == p2.Id))
            {
                context.Patients.Add(new Patient { UserId = p2.Id, FullName = p2.Name });
            }
            if (!await context.Patients.AnyAsync(p => p.UserId == p3.Id))
            {
                context.Patients.Add(new Patient { UserId = p3.Id, FullName = p3.Name });
            }
            if (!await context.Patients.AnyAsync(p => p.UserId == p4.Id))
            {
                context.Patients.Add(new Patient { UserId = p4.Id, FullName = p4.Name });
            }
            if (!await context.Patients.AnyAsync(p => p.UserId == p5.Id))
            {
                context.Patients.Add(new Patient { UserId = p5.Id, FullName = p5.Name });
            }
            if (!await context.Patients.AnyAsync(p => p.UserId == p6.Id))
            {
                context.Patients.Add(new Patient { UserId = p6.Id, FullName = p6.Name });
            }
            if (!await context.Patients.AnyAsync(p => p.UserId == p7.Id))
            {
                context.Patients.Add(new Patient { UserId = p7.Id, FullName = p7.Name });
            }
            await context.SaveChangesAsync();

            // Create sample appointments (2 per doctor)
            if (!await context.Appointments.AnyAsync())
            {
                var doctors = await context.Doctors.Include(d => d.Services).ToListAsync();
                var patients = await context.Patients.ToListAsync();
                var baseDate = new DateTime(2026, 6, 1);

                // Doctor 1 (Ivan - Therapist)
                if (doctors.Count > 0 && doctors[0].Services.Count > 0)
                {
                    var doc = doctors[0];
                    var svc1 = doc.Services.First();
                    var svc2 = doc.Services.Count > 1 ? doc.Services.Last() : svc1;
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(9), EndAt = baseDate.AddHours(9.5), DoctorId = doc.DoctorId, PatientId = patients[0].PatientId, ServiceId = svc1.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(10.5), EndAt = baseDate.AddHours(11.5), DoctorId = doc.DoctorId, PatientId = patients[1].PatientId, ServiceId = svc2.ServiceId });
                }

                // Doctor 2 (Anna - Therapist)
                if (doctors.Count > 1 && doctors[1].Services.Count > 0)
                {
                    var doc = doctors[1];
                    var svc = doc.Services.First();
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(12), EndAt = baseDate.AddHours(12.5), DoctorId = doc.DoctorId, PatientId = patients[2].PatientId, ServiceId = svc.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(13), EndAt = baseDate.AddHours(13.5), DoctorId = doc.DoctorId, PatientId = patients[3].PatientId, ServiceId = svc.ServiceId });
                }

                // Doctor 3 (Maria - Cardiologist)
                if (doctors.Count > 2 && doctors[2].Services.Count > 0)
                {
                    var doc = doctors[2];
                    var svc1 = doc.Services.First();
                    var svc2 = doc.Services.Count > 1 ? doc.Services.Last() : svc1;
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(9), EndAt = baseDate.AddHours(9.75), DoctorId = doc.DoctorId, PatientId = patients[4].PatientId, ServiceId = svc1.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(10.5), EndAt = baseDate.AddHours(11.5), DoctorId = doc.DoctorId, PatientId = patients[5].PatientId, ServiceId = svc2.ServiceId });
                }

                // Doctor 4 (Sergiy - Cardiologist)
                if (doctors.Count > 3 && doctors[3].Services.Count > 0)
                {
                    var doc = doctors[3];
                    var svc = doc.Services.First();
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(12), EndAt = baseDate.AddHours(12.75), DoctorId = doc.DoctorId, PatientId = patients[6].PatientId, ServiceId = svc.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(13.5), EndAt = baseDate.AddHours(14.25), DoctorId = doc.DoctorId, PatientId = patients[0].PatientId, ServiceId = svc.ServiceId });
                }

                // Doctor 5 (Olga - Dentist)
                if (doctors.Count > 4 && doctors[4].Services.Count > 0)
                {
                    var doc = doctors[4];
                    var svc1 = doc.Services.First();
                    var svc2 = doc.Services.Count > 1 ? doc.Services.Last() : svc1;
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(9), EndAt = baseDate.AddHours(9.5), DoctorId = doc.DoctorId, PatientId = patients[1].PatientId, ServiceId = svc1.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(10.5), EndAt = baseDate.AddHours(11.25), DoctorId = doc.DoctorId, PatientId = patients[2].PatientId, ServiceId = svc2.ServiceId });
                }

                // Doctor 6 (Pavlo - Dentist)
                if (doctors.Count > 5 && doctors[5].Services.Count > 0)
                {
                    var doc = doctors[5];
                    var svc = doc.Services.First();
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(12), EndAt = baseDate.AddHours(12.5), DoctorId = doc.DoctorId, PatientId = patients[3].PatientId, ServiceId = svc.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(13.5), EndAt = baseDate.AddHours(14), DoctorId = doc.DoctorId, PatientId = patients[4].PatientId, ServiceId = svc.ServiceId });
                }

                // Doctor 7 (Petro - Neurologist)
                if (doctors.Count > 6 && doctors[6].Services.Count > 0)
                {
                    var doc = doctors[6];
                    var svc1 = doc.Services.First();
                    var svc2 = doc.Services.Count > 1 ? doc.Services.Last() : svc1;
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(9), EndAt = baseDate.AddHours(10), DoctorId = doc.DoctorId, PatientId = patients[5].PatientId, ServiceId = svc1.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(11), EndAt = baseDate.AddHours(12.5), DoctorId = doc.DoctorId, PatientId = patients[6].PatientId, ServiceId = svc2.ServiceId });
                }

                // Doctor 8 (Liudmyla - Neurologist)
                if (doctors.Count > 7 && doctors[7].Services.Count > 0)
                {
                    var doc = doctors[7];
                    var svc = doc.Services.First();
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(13), EndAt = baseDate.AddHours(14), DoctorId = doc.DoctorId, PatientId = patients[0].PatientId, ServiceId = svc.ServiceId });
                    context.Appointments.Add(new Appointment { StartAt = baseDate.AddHours(14.5), EndAt = baseDate.AddHours(16), DoctorId = doc.DoctorId, PatientId = patients[1].PatientId, ServiceId = svc.ServiceId });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
