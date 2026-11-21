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
            var tomorrowLate = now.Date.AddDays(1).AddHours(23);   // 23:00 UTC tomorrow
            var sessions = new List<Session>();

            const string karim = "karim.fawzy@example.com";   // active sessions  -> trainer delete BLOCKED
            const string mona = "mona.rashad@example.com";    // active sessions  -> trainer delete BLOCKED
            const string hossam = "hossam.nabil@example.com"; // completed only   -> trainer delete allowed
                                                              // dina.salah@example.com: no sessions at all       -> trainer delete allowed

            void Add(string description, string category, string trainerEmail,
                        DateTime start, int durationMinutes, int capacity)
            {
                if (!trainers.TryGetValue(trainerEmail, out var trainerId)) return;
                if (!categories.TryGetValue(category, out var categoryId)) return;

                sessions.Add(new Session
                {
                    Description = description,          // must be unique: BookingSeeder keys on it
                    Capacity = capacity,                // CK: 1..25
                    StartDate = start,
                    EndDate = start.AddMinutes(durationMinutes),
                    TrainerId = trainerId,
                    CategoryId = categoryId
                });
            }

            // ---------- COMPLETED ----------
            Add("Boxing Fundamentals", "Boxing", karim, now.AddHours(-72), 60, 15);
            Add("Morning Yoga Flow", "Yoga", mona, now.AddHours(-48), 60, 20);
            Add("CrossFit WOD", "CrossFit", hossam, now.AddHours(-24), 60, 12);
            Add("CrossFit Express (Just Finished)", "CrossFit", hossam, now.AddMinutes(-35), 30, 10);  // ended 5 min ago
            Add("Pilates Intro (No Attendance)", "Pilates", hossam, now.AddHours(-96), 60, 10);        // completed, zero bookings

            // ---------- ONGOING ----------
            Add("Boxing Cardio Blast", "Boxing", karim, now.AddMinutes(-30), 90, 15);                  // ends in 60 min
            Add("Power Yoga (Just Started)", "Yoga", mona, now.AddMinutes(-5), 60, 20);               // ends in 55 min
            Add("Cardio Burn (Ending Soon)", "Cardio", karim, now.AddMinutes(-55), 60, 8);            // ends in 5 min

            // ---------- UPCOMING ----------
            Add("Spin Class (Starting Soon)", "Cycling", mona, now.AddMinutes(15), 45, 10);
            Add("Evening Yoga", "Yoga", mona, now.AddHours(3), 60, 20);
            Add("HIIT Bootcamp", "Cardio", karim, now.AddHours(36), 60, 4);                           // FULL 4/4
            Add("Boxing Technique", "Boxing", karim, now.AddHours(48), 60, 10);
            Add("Pilates Basics", "Pilates", mona, now.AddHours(72), 60, 3);                          // ONE SLOT LEFT 2/3
            Add("Yoga for Beginners", "Yoga", mona, now.AddHours(96), 60, 2);                         // FULL 2/2
            Add("Strength Foundations", "Strength Training", karim, now.AddHours(120), 75, 6);        // HALF FULL 3/6
            Add("Private Cardio Session", "Cardio", mona, now.AddDays(7), 30, 1);                     // FULL 1/1 (min capacity)
            Add("Late Night Strength", "Strength Training", karim, tomorrowLate, 90, 25);             // crosses midnight, max capacity, EMPTY
            Add("Zumba Party", "Zumba", mona, now.AddDays(14), 120, 20);                              // 2h duration, EMPTY

            await dbContext.Sessions.AddRangeAsync(sessions);
            await dbContext.SaveChangesAsync();
        }
    }
}
