using GymManagementSystem_BLL.ViewModels.Common;

namespace GymManagementSystem_BLL.ViewModels.Sessions
{
    public record SessionLookups(IReadOnlyList<LookupItem> Categories,
        IReadOnlyList<LookupItem> Trainers);
}
