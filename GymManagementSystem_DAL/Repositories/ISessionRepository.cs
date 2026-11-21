using GymManagementSystem_DAL.Models;

namespace GymManagementSystem_DAL.Repositories
{
    public interface ISessionRepository
    {
        Task<IReadOnlyList<Session>> GetAllWithDetailsAsync(CancellationToken ct = default);
    }
}
