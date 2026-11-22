using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    public class SessionsController(ISessionService sessions) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await sessions.GetAllAsync(ct));

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
            => await sessions.GetDetailsAsync(id, ct) switch
            {
                SessionViewModel session => View(session),
                Error => NotFound()
            };
    }
}
