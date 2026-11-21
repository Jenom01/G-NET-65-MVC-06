using GymManagementSystem_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_DAL.Data.Seed
{
    public static class MembershipSeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Memberships.AnyAsync()) return;

            var members = await dbContext.Set<Member>()
                .AsNoTracking()
                .ToDictionaryAsync(m => m.Email, m => m.Id);

            var plans = await dbContext.Plans
                .AsNoTracking()
                .ToDictionaryAsync(p => p.Name, p => new { p.Id, p.DurationDays });

            var now = DateTime.UtcNow;
            var memberships = new List<Membership>();

            // startOffsetDays is negative = started in the past; EndDate comes from the plan's duration
            void Add(string email, string planName, int startOffsetDays)
            {
                if (!members.TryGetValue(email, out var memberId)) return;
                if (!plans.TryGetValue(planName, out var plan)) return;

                var start = now.AddDays(startOffsetDays);

                memberships.Add(new Membership
                {
                    MemberId = memberId,
                    PlanId = plan.Id,
                    StartDate = start,
                    EndDate = start.AddDays(plan.DurationDays)
                });
            }

            // Ahmed: expired Quarterly, then an active Annual
            Add("ahmed.hassan@example.com", "Quarterly", -180);   // ended ~90 days ago
            Add("ahmed.hassan@example.com", "Annual", -60);       // active

            // Sara: expired Monthly, then an active Monthly (renewal)
            Add("sara.mahmoud@example.com", "Monthly", -120);     // expired
            Add("sara.mahmoud@example.com", "Monthly", -10);      // active

            // Omar: active Quarterly
            Add("omar.khaled@example.com", "Quarterly", -30);     // active

            // Nour: expired Weekly Pass, then an active Monthly
            Add("nour.ibrahim@example.com", "Weekly Pass", -20);  // expired
            Add("nour.ibrahim@example.com", "Monthly", -3);       // active

            // Youssef: only an expired Weekly Pass (no active membership)
            Add("youssef.adel@example.com", "Weekly Pass", -40);  // expired

            // Mariam: intentionally no membership (tests the "no membership" case)

            await dbContext.Memberships.AddRangeAsync(memberships);
            await dbContext.SaveChangesAsync();
        }
    }
}
