using AutoMapper;
using GymManagementSystem_BLL.Errors;
using GymManagementSystem_BLL.Helpers;
using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.ViewModels.Common;
using GymManagementSystem_BLL.ViewModels.Sessions;
using GymManagementSystem_DAL.Enums;
using GymManagementSystem_DAL.Models;
using GymManagementSystem_DAL.Repositories;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem_BLL.Services
{
    public class SessionService(IUnitOfWork uow, IMapper mapper, ILogger<SessionService> logger) : ISessionService
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

        public async Task<SessionLookups> GetLookupsAsync(CancellationToken ct = default)
        {
            var categories = await uow.Categories.GetAllAsync(ct);
            var trainers = await uow.Trainers.GetAllAsync(ct);

            return new SessionLookups(
                categories.OrderBy(c => c.Name).Select(c => new LookupItem(c.Id, c.Name)).ToList(),
                trainers.OrderBy(t => t.Name)
                        .Select(t => new LookupItem(t.Id, $"{t.Name} ({t.Speciality.GetDisplayName()})"))
                        .ToList());
        }

        public async Task<Result> CreateAsync(SessionCreateViewModel model, CancellationToken ct = default)
        {
            // 1. The form sends Cairo wall-clock time with no zone. The DB stores UTC.
            var startLocal = DateTime.SpecifyKind(model.StartDate!.Value, DateTimeKind.Unspecified);
            var endLocal = DateTime.SpecifyKind(model.EndDate!.Value, DateTimeKind.Unspecified);

            if (endLocal <= startLocal) return Reject(SessionErrors.InvalidDateRange, model);

            if (GymTimeZone.IsInvalidTime(startLocal) || GymTimeZone.IsInvalidTime(endLocal))
                return Reject(SessionErrors.InvalidLocalTime, model);

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, GymTimeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, GymTimeZone);

            // 2. Start must be in the future (compared in UTC)
            if (startUtc <= DateTime.UtcNow) return Reject(SessionErrors.StartInPast, model);

            // 3. Lookups exist
            var category = await uow.Categories.GetByIdAsync(model.CategoryId!.Value, ct);
            if (category is null) return Reject(SessionErrors.CategoryNotFound, model);

            var trainer = await uow.Trainers.GetByIdAsync(model.TrainerId!.Value, ct);
            if (trainer is null) return Reject(SessionErrors.TrainerNotFound, model);

            // 4. Specialty must match category (Category.Name is "Strength Training", the enum's Display name)
            if (!string.Equals(trainer.Speciality.GetDisplayName(), category.Name, StringComparison.OrdinalIgnoreCase))
                return Reject(SessionErrors.SpecialityMismatch, model);

            // 5. No double-booking
            if (await uow.Sessions.HasTrainerConflictAsync(trainer.Id, startUtc, endUtc, ct: ct))
                return Reject(SessionErrors.TrainerBusy, model);

            // 6. Save
            var session = mapper.Map<Session>(model);
            session.StartDate = startUtc;
            session.EndDate = endUtc;

            uow.Sessions.Add(session);
            await uow.SaveChangesAsync(ct);

            logger.LogInformation(
                "Session {SessionId} created: trainer {TrainerId}, category {CategoryId}, {StartUtc:u} to {EndUtc:u}",
                session.Id, session.TrainerId, session.CategoryId, startUtc, endUtc);

            return new Ok();
        }

        private SessionViewModel ToViewModel(Session session, DateTime nowUtc)
        {
            var model = mapper.Map<SessionViewModel>(session);

            model.Status = session.GetStatus(nowUtc);
            model.StartDate = TimeZoneInfo.ConvertTimeFromUtc(session.StartDate, GymTimeZone);
            model.EndDate = TimeZoneInfo.ConvertTimeFromUtc(session.EndDate, GymTimeZone);

            return model;
        }

        private Error Reject(Error error, SessionCreateViewModel model)
        {
            logger.LogInformation(
                "Session create rejected: {ErrorCode} (trainer {TrainerId}, category {CategoryId})",
                error.Code, model.TrainerId, model.CategoryId);
            return error;
        }
    }
}
