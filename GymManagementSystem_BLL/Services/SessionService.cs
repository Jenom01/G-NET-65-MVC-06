using AutoMapper;
using GymManagementSystem_BLL.ViewModels.Sessions;
using GymManagementSystem_DAL.Enums;
using GymManagementSystem_DAL.Repositories;

namespace GymManagementSystem_BLL.Services
{
    public class SessionService(IUnitOfWork uow, IMapper mapper) : ISessionService
    {
        public async Task<IEnumerable<SessionViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var sessions = await uow.Sessions.GetAllWithDetailsAsync(ct);
            var now = DateTime.UtcNow;
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

            var list = mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var (vm, session) in list.Zip(sessions))
            {
                vm.Status = session.GetStatus(now);
                vm.StartDate = TimeZoneInfo.ConvertTimeFromUtc(session.StartDate, tz);
                vm.EndDate = TimeZoneInfo.ConvertTimeFromUtc(session.EndDate, tz);
            }

            list = list.OrderBy(v => v.Status == SessionStatus.Completed)
                       .ThenBy(v => v.StartDate)
                       .ToList();

            return list;
        }
    }
}
