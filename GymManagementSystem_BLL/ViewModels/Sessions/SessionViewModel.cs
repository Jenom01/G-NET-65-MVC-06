using GymManagementSystem_DAL.Enums;

namespace GymManagementSystem_BLL.ViewModels.Sessions
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = default!;
        public string TrainerName { get; set; } = default!;
        public string CategoryName { get; set; } = default!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public SessionStatus Status { get; set; }

        public int AvailableSlots => Math.Max(Capacity - BookedCount, 0);
        public bool IsFull => AvailableSlots == 0;

        public string DateDisplay
            => $"{StartDate:MMM dd, yyyy}";

        public string TimeRangeDisplay
            => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";

        public string DurationDisplay
        {
            get
            {
                var d = EndDate - StartDate;
                return d.TotalHours >= 1 ? $"{(int)d.TotalHours}h {d.Minutes}m" : $"{d.Minutes}m";
            }
        }
    }
}
