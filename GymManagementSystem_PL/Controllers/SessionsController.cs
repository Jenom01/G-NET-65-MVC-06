using GymManagementSystem_BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    public class SessionsController(ISessionService sessions) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await sessions.GetAllAsync(ct));
    }
}
