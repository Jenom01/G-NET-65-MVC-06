using GymManagementSystem_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_DAL.Data.Seed
{
    public static class CategorySeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Categories.AnyAsync()) return;

            var categories = new List<Category>
            {
                new() { Name = "Boxing" },
                new() { Name = "Yoga" },
                new() { Name = "CrossFit" },
                new() { Name = "Pilates" },
                new() { Name = "Zumba" },
                new() { Name = "Cycling" },
                new() { Name = "Strength Training" },
                new() { Name = "Cardio" }
            };

            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
        }
    }
}
