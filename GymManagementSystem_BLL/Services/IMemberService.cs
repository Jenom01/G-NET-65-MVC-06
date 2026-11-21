using GymManagementSystem_BLL.ViewModels.HealthRecords;
using GymManagementSystem_BLL.ViewModels.Members;

namespace GymManagementSystem_BLL.Services
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberIndexViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<MemberResult> CreateAsync(MemberCreateViewModel model, CancellationToken ct = default);
        Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default);
        Task<HealthRecordDetailsViewModel?> GetHealthRecordAsync(int memberId, CancellationToken ct = default);
        Task<MemberEditViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default);
        Task<MemberResult> UpdateAsync(int id, MemberEditViewModel model, CancellationToken ct = default);
        Task<MemberResult> DeleteAsync(int id, CancellationToken ct = default);
    }
}
