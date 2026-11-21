using GymManagementSystem_BLL.ViewModels.Trainers;

namespace GymManagementSystem_BLL.Services
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<TrainerResult> CreateAsync(TrainerCreateViewModel model, CancellationToken ct = default);
        Task<TrainerDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default);
        Task<TrainerEditViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default);
        Task<TrainerResult> UpdateAsync(int id, TrainerEditViewModel model, CancellationToken ct = default);
        Task<TrainerResult> DeleteAsync(int id, CancellationToken ct = default);
    }
}
