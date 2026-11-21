using GymManagementSystem_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_DAL.Data.Seed
{
    public static class PlanSeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Plans.AnyAsync()) return;

            List<Plan> plans = new()
            {
                new Plan
                {
                    Name = "Monthly",
                    Description = "Full gym access for 30 days. Ideal for trying us out.",
                    DurationDays = 30,
                    Price = 500m,
                    IsActive = true,
                },
                new Plan
                {
                    Name = "Quarterly",
                    Description = "Full gym access for 3 months with group classes included.",
                    DurationDays = 90,
                    Price = 1350m,
                    IsActive = true,
                },
                new Plan
                {
                    Name = "Semi-Annual",
                    Description = "6 months of full access, group classes, and 2 personal training sessions.",
                    DurationDays = 180,
                    Price = 2500m,
                    IsActive = true,
                },
                new Plan
                {
                    Name = "Annual",
                    Description = "12 months of full access, unlimited classes, and 4 personal training sessions. Best value.",
                    DurationDays = 365,
                    Price = 4500m,
                    IsActive = true,
                },
                new Plan
                {
                    Name = "Student",
                    Description = "Discounted monthly plan for students with a valid ID. Off-peak hours only.",
                    DurationDays = 30,
                    Price = 300m,
                    IsActive = true,
                },
                new Plan
                {
                    Name = "Weekly Pass",
                    Description = "7 days of full access. Perfect for visitors.",
                    DurationDays = 7,
                    Price = 150m,
                    IsActive = false,
                }
            };

            await dbContext.Plans.AddRangeAsync(plans);
            await dbContext.SaveChangesAsync();
        }
    }
}
