using AutoMapper;
using GymManagementSystem_BLL.Errors;
using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.ViewModels.Sessions;
using GymManagementSystem_DAL.Enums;
using GymManagementSystem_DAL.Models;
using GymManagementSystem_DAL.Repositories;

namespace GymManagementSystem_BLL.Services
{
    public class SessionService(IUnitOfWork uow, IMapper mapper) : ISessionService
    {
        private static readonly TimeZoneInfo GymTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

        public async Task<IEnumerable<SessionViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var sessions = await uow.Sessions.GetAllWithDetailsAsync(ct);
            var now = DateTime.UtcNow;

            return sessions
                .Select(s => ToViewModel(s, now))
                .OrderBy(v => v.Status == SessionStatus.Completed)
                .ThenBy(v => v.StartDate)
                .ToList();
        }

        public async Task<Result<SessionViewModel>> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var session = await uow.Sessions.GetDetailsAsync(id, ct);

            if (session is null) return SessionErrors.NotFound;

            return ToViewModel(session, DateTime.UtcNow);
        }

        private SessionViewModel ToViewModel(Session session, DateTime nowUtc)
        {
            var model = mapper.Map<SessionViewModel>(session);

            model.Status = session.GetStatus(nowUtc);
            model.StartDate = TimeZoneInfo.ConvertTimeFromUtc(session.StartDate, GymTimeZone);
            model.EndDate = TimeZoneInfo.ConvertTimeFromUtc(session.EndDate, GymTimeZone);

            return model;
        }
    }
}
