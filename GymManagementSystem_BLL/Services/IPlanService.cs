using GymManagementSystem_BLL.ViewModels.Plans;

namespace GymManagementSystem_BLL.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetDetailsAsync(int id, CancellationToken ct = default);
        Task<PlanEditViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default);
        Task<PlanResult> UpdateAsync(int id, PlanEditViewModel model, CancellationToken ct = default);
        Task<PlanResult> ToggleStatusAsync(int id, CancellationToken ct = default);
    }
}
