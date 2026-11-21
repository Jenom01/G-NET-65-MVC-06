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

            void Add(string email, string sessionDescription)
            {
                if (!members.TryGetValue(email, out var memberId)) return;
                if (!sessions.TryGetValue(sessionDescription, out var session)) return;

                bookings.Add(new Booking
                {
                    MemberId = memberId,
                    SessionId = session.Id,
                    BookingDate = session.StartDate.AddDays(-1),
                    IsAttended = session.EndDate < now      // attended only if already finished
                });
            }

            // Ahmed: past + 2 upcoming        -> has ACTIVE bookings
            Add("ahmed.hassan@example.com", "Boxing Fundamentals");
            Add("ahmed.hassan@example.com", "Evening Yoga");
            Add("ahmed.hassan@example.com", "Yoga for Beginners");

            // Sara: past + ONGOING            -> has ACTIVE booking
            Add("sara.mahmoud@example.com", "Morning Yoga Flow");
            Add("sara.mahmoud@example.com", "Boxing Cardio Blast");

            // Omar: only a past booking       -> NO active bookings (delete allowed)
            Add("omar.khaled@example.com", "CrossFit WOD");

            // Nour: 2 upcoming                -> has ACTIVE bookings (fills "Yoga for Beginners")
            Add("nour.ibrahim@example.com", "Boxing Technique");
            Add("nour.ibrahim@example.com", "Yoga for Beginners");

            // Youssef, Mariam: none           -> delete allowed

            await dbContext.Bookings.AddRangeAsync(bookings);
            await dbContext.SaveChangesAsync();
        }
    }
}
