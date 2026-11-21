using AutoMapper;
using GymManagementSystem_BLL.Errors;
using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.ViewModels.Plans;
using GymManagementSystem_DAL.Repositories;

namespace GymManagementSystem_BLL.Services
{
    public class PlanService(IUnitOfWork uow, IMapper mapper) : IPlanService
    {
        public async Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default)
            => mapper.Map<IEnumerable<PlanViewModel>>(await uow.Plans.GetAllAsync(ct));

        public async Task<Result<PlanViewModel>> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);

            if (plan is null) return PlanErrors.NotFound;

            return mapper.Map<PlanViewModel>(plan);
        }

        public async Task<Result<PlanEditViewModel>> GetForUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);

            if (plan is null) return PlanErrors.NotFound;

            return mapper.Map<PlanEditViewModel>(plan);
        }

        public async Task<Result> UpdateAsync(int id, PlanEditViewModel model, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);

            if (plan is null) return PlanErrors.NotFound;

            if (await uow.Plans.HasActiveMembershipsAsync(id, ct))
                return PlanErrors.CannotUpdateWithActiveMemberships;

            mapper.Map(model, plan);
            await uow.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await uow.Plans.GetByIdAsync(id, ct);

            if (plan is null) return PlanErrors.NotFound;

            if (plan.IsActive && await uow.Plans.HasActiveMembershipsAsync(id, ct))
                return PlanErrors.CannotDeactivateWithActiveMemberships;

            plan.IsActive = !plan.IsActive;
            await uow.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
