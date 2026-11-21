using GymManagementSystem_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_DAL.Data.Seed
{
    public static class BookingSeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Bookings.AnyAsync()) return;

            var members = await dbContext.Set<Member>().AsNoTracking()
                .ToDictionaryAsync(m => m.Email, m => m.Id);
            var sessions = await dbContext.Sessions.AsNoTracking()
                .ToDictionaryAsync(s => s.Description);

            var now = DateTime.UtcNow;
            var bookings = new List<Booking>();

            void Add(string email, string sessionDescription, bool? attended = null)
            {
                if (!members.TryGetValue(email, out var memberId)) return;
                if (!sessions.TryGetValue(sessionDescription, out var session)) return;

                bookings.Add(new Booking
                {
                    MemberId = memberId,
                    SessionId = session.Id,
                    BookingDate = session.StartDate.AddDays(-1),
                    IsAttended = attended ?? session.EndDate < now   // default: attended only if finished
                });
            }

            const string ahmed = "ahmed.hassan@example.com";
            const string sara = "sara.mahmoud@example.com";
            const string omar = "omar.khaled@example.com";
            const string nour = "nour.ibrahim@example.com";
            const string mariam = "mariam.samir@example.com";
            // youssef.adel@example.com: no bookings -> delete allowed

            // ----- Completed -----
            Add(ahmed, "Boxing Fundamentals");
            Add(sara, "Morning Yoga Flow");
            Add(omar, "CrossFit WOD");                                   // Omar: past only -> delete allowed
            Add(omar, "CrossFit Express (Just Finished)", attended: false);   // no-show

            // ----- Ongoing (active) -----
            Add(sara, "Boxing Cardio Blast");
            Add(ahmed, "Power Yoga (Just Started)");
            Add(nour, "Cardio Burn (Ending Soon)");

            // ----- Upcoming (active) -----
            Add(sara, "Spin Class (Starting Soon)");
            Add(ahmed, "Evening Yoga");
            Add(nour, "Boxing Technique");

            Add(ahmed, "Yoga for Beginners");                            // FULL 2/2
            Add(nour, "Yoga for Beginners");

            Add(mariam, "Private Cardio Session");                       // FULL 1/1

            Add(ahmed, "HIIT Bootcamp");                                 // FULL 4/4
            Add(sara, "HIIT Bootcamp");
            Add(nour, "HIIT Bootcamp");
            Add(mariam, "HIIT Bootcamp");

            Add(sara, "Pilates Basics");                                 // ONE SLOT LEFT 2/3
            Add(nour, "Pilates Basics");

            Add(ahmed, "Strength Foundations");                          // HALF FULL 3/6
            Add(sara, "Strength Foundations");
            Add(mariam, "Strength Foundations");

            // ----- Guard: never exceed capacity -----
            foreach (var group in bookings.GroupBy(b => b.SessionId))
            {
                var session = sessions.Values.First(s => s.Id == group.Key);
                if (group.Count() > session.Capacity)
                    throw new InvalidOperationException(
                        $"Seed error: '{session.Description}' has {group.Count()} bookings for capacity {session.Capacity}.");
            }

            await dbContext.Bookings.AddRangeAsync(bookings);
            await dbContext.SaveChangesAsync();
        }
    }
}
