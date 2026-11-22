using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.ViewModels.Sessions;

namespace GymManagementSystem_BLL.Services
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<Result<SessionViewModel>> GetDetailsAsync(int id, CancellationToken ct = default);
    }
}
