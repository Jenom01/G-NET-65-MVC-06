using GymManagementSystem_DAL.Enums;
using GymManagementSystem_DAL.Models;
using GymManagementSystem_DAL.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_DAL.Data.Seed
{
    public static class MemberSeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Set<Member>().AnyAsync()) return;

            var members = new List<Member>
            {
                new()
                {
                    Name = "Ahmed Hassan",
                    Email = "ahmed.hassan@example.com",
                    Phone = "01012345678",
                    DateOfBirth = new DateOnly(1998, 3, 14),
                    Gender = Gender.Male,
                    Address = new Address { City = "Cairo", Street = "Tahrir St", BuildingNumber = 12 },
                    JoinDate = DateTime.UtcNow.AddMonths(-6),
                    HealthRecord = new HealthRecord
                    {
                        Height = 178m, Weight = 82.5m, BloodType = BloodType.OPositive,
                        Note = "No known conditions."
                    }
                },
                new()
                {
                    Name = "Sara Mahmoud",
                    Email = "sara.mahmoud@example.com",
                    Phone = "01112345678",
                    DateOfBirth = new DateOnly(2000, 7, 2),
                    Gender = Gender.Female,
                    Address = new Address { City = "Giza", Street = "Pyramids Rd", BuildingNumber = 45 },
                    JoinDate = DateTime.UtcNow.AddMonths(-4),
                    HealthRecord = new HealthRecord
                    {
                        Height = 165m, Weight = 58m, BloodType = BloodType.APositive,
                        Note = "Mild asthma."
                    }
                },
                new()
                {
                    Name = "Omar Khaled",
                    Email = "omar.khaled@example.com",
                    Phone = "01212345678",
                    DateOfBirth = new DateOnly(1995, 11, 30),
                    Gender = Gender.Male,
                    Address = new Address { City = "Alexandria", Street = "Corniche Rd", BuildingNumber = 7 },
                    JoinDate = DateTime.UtcNow.AddMonths(-3),
                    HealthRecord = new HealthRecord
                    {
                        Height = 183m, Weight = 90m, BloodType = BloodType.BPositive
                    }
                },
                new()
                {
                    Name = "Nour Ibrahim",
                    Email = "nour.ibrahim@example.com",
                    Phone = "01512345678",
                    DateOfBirth = new DateOnly(2002, 1, 21),
                    Gender = Gender.Female,
                    Address = new Address { City = "Assiut", Street = "Al Gomhoreya St", BuildingNumber = 3 },
                    JoinDate = DateTime.UtcNow.AddMonths(-2),
                    HealthRecord = new HealthRecord
                    {
                        Height = 170m, Weight = 62m, BloodType = BloodType.ABPositive
                    }
                },
                new()
                {
                    Name = "Youssef Adel",
                    Email = "youssef.adel@example.com",
                    Phone = "01023456789",
                    DateOfBirth = new DateOnly(1990, 5, 9),
                    Gender = Gender.Male,
                    Address = new Address { City = "Mansoura", Street = "El Geish St", BuildingNumber = 28 },
                    JoinDate = DateTime.UtcNow.AddMonths(-1),
                    HealthRecord = new HealthRecord
                    {
                        Height = 175m, Weight = 77m, BloodType = BloodType.ONegative,
                        Note = "Previous knee injury, avoid heavy squats."
                    }
                },
                new()
                {
                    Name = "Mariam Samir",
                    Email = "mariam.samir@example.com",
                    Phone = "01123456789",
                    DateOfBirth = new DateOnly(1999, 9, 17),
                    Gender = Gender.Female,
                    Address = new Address { City = "Cairo", Street = "Maadi Rd", BuildingNumber = 60 },
                    HealthRecord = new HealthRecord
                    {
                        Height = 160m, Weight = 55m, BloodType = BloodType.ANegative
                    }
                }
            };

            await dbContext.Set<Member>().AddRangeAsync(members);
            await dbContext.SaveChangesAsync();
        }
    }
}
