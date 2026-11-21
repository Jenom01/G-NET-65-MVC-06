using GymManagementSystem_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_DAL.Data.Seed
{
    public static class SessionSeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Sessions.AnyAsync()) return;

            var trainers = await dbContext.Set<Trainer>().AsNoTracking()
                .ToDictionaryAsync(t => t.Email, t => t.Id);
            var categories = await dbContext.Categories.AsNoTracking()
                .ToDictionaryAsync(c => c.Name, c => c.Id);

            var now = DateTime.UtcNow;
            var sessions = new List<Session>();

            void Add(string description, string category, string trainerEmail,
                     double startOffsetHours, int durationMinutes, int capacity)
            {
                if (!trainers.TryGetValue(trainerEmail, out var trainerId)) return;
                if (!categories.TryGetValue(category, out var categoryId)) return;

                var start = now.AddHours(startOffsetHours);
                sessions.Add(new Session
                {
                    Description = description,
                    Capacity = capacity,
                    StartDate = start,
                    EndDate = start.AddMinutes(durationMinutes),
                    TrainerId = trainerId,
                    CategoryId = categoryId
                });
            }

            // Completed
            Add("Boxing Fundamentals", "Boxing", "karim.fawzy@example.com", -72, 60, 15);
            Add("Morning Yoga Flow", "Yoga", "mona.rashad@example.com", -48, 60, 20);
            Add("CrossFit WOD", "CrossFit", "hossam.nabil@example.com", -24, 60, 12);

            // Ongoing (started 30 min ago, ends in 60 min)
            Add("Boxing Cardio Blast", "Boxing", "karim.fawzy@example.com", -0.5, 90, 15);

            // Upcoming
            Add("Evening Yoga", "Yoga", "mona.rashad@example.com", 3, 60, 20);
            Add("Boxing Technique", "Boxing", "karim.fawzy@example.com", 48, 60, 10);
            Add("Yoga for Beginners", "Yoga", "mona.rashad@example.com", 96, 60, 2);   // small capacity: fills up

            await dbContext.Sessions.AddRangeAsync(sessions);
            await dbContext.SaveChangesAsync();
        }
    }
}
