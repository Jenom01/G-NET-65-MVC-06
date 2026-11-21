using AutoMapper;
using GymManagementSystem_BLL.ViewModels.Plans;
using GymManagementSystem_DAL.Repositories;

namespace GymManagementSystem_BLL.Services
{
    public class PlanService(IUnitOfWork uow, IMapper mapper) : IPlanService
    {
        public async Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default)
            => mapper.Map<IEnumerable<PlanViewModel>>(await uow.Plans.GetAllAsync(ct));

        public async Task<PlanViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);
            return plan is null ? null : mapper.Map<PlanViewModel>(plan);
        }

        public async Task<PlanEditViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);
            return plan is null ? null : mapper.Map<PlanEditViewModel>(plan);
        }

        public async Task<PlanResult> UpdateAsync(int id, PlanEditViewModel model, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);

            if (plan is null) return PlanResult.NotFound;

            if (await uow.Plans.HasActiveMembershipsAsync(id, ct))
                return PlanResult.HasActiveMemberships;

            mapper.Map(model, plan);
            await uow.SaveChangesAsync(ct);

            return PlanResult.Success;
        }

        public async Task<PlanResult> ToggleStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);

            if (plan is null) return PlanResult.NotFound;

            if (plan.IsActive && await uow.Plans.HasActiveMembershipsAsync(id, ct))
                return PlanResult.HasActiveMemberships;

            plan.IsActive = !plan.IsActive;
            await uow.SaveChangesAsync(ct);

            return PlanResult.Success;
        }
    }
}
