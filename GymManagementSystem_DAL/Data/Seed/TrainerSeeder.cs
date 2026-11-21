using GymManagementSystem_DAL.Enums;
using GymManagementSystem_DAL.Models;
using GymManagementSystem_DAL.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_DAL.Data.Seed
{
    public static class TrainerSeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Set<Trainer>().AnyAsync()) return;

            var trainers = new List<Trainer>
            {
                new()
                {
                    Name = "Karim Fawzy", Email = "karim.fawzy@example.com", Phone = "01098765432",
                    DateOfBirth = new DateOnly(1989, 4, 12), Gender = Gender.Male,
                    Address = new Address { City = "Cairo", Street = "Nasr City Rd", BuildingNumber = 15 },
                    Speciality = Speciality.Boxing, HireDate = DateTime.UtcNow.AddYears(-3)
                },
                new()
                {
                    Name = "Mona Rashad", Email = "mona.rashad@example.com", Phone = "01198765432",
                    DateOfBirth = new DateOnly(1992, 8, 25), Gender = Gender.Female,
                    Address = new Address { City = "Giza", Street = "Dokki St", BuildingNumber = 9 },
                    Speciality = Speciality.Yoga, HireDate = DateTime.UtcNow.AddYears(-2)
                },
                new()
                {
                    Name = "Hossam Nabil", Email = "hossam.nabil@example.com", Phone = "01298765432",
                    DateOfBirth = new DateOnly(1991, 1, 30), Gender = Gender.Male,
                    Address = new Address { City = "Alexandria", Street = "Smouha St", BuildingNumber = 22 },
                    Speciality = Speciality.CrossFit, HireDate = DateTime.UtcNow.AddMonths(-10)
                },
                new()
                {
                    Name = "Dina Salah", Email = "dina.salah@example.com", Phone = "01598765432",
                    DateOfBirth = new DateOnly(1995, 12, 3), Gender = Gender.Female,
                    Address = new Address { City = "Cairo", Street = "Zamalek St", BuildingNumber = 4 },
                    Speciality = Speciality.Pilates, HireDate = DateTime.UtcNow.AddMonths(-5)
                }
            };

            await dbContext.Set<Trainer>().AddRangeAsync(trainers);
            await dbContext.SaveChangesAsync();
        }
    }
}
