using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Common;
using GymManagementSystem_BLL.ViewModels.Sessions;
using GymManagementSystem_PL.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await LoadLookupsAsync(ct);
            return View(new SessionCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionCreateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(ct);
                return View(model);
            }

            var result = await sessions.CreateAsync(model, ct);

            switch (result)
            {
                case Error error:
                    ModelState.AddError(error);
                    await LoadLookupsAsync(ct);
                    return View(model);

                default:
                    TempData["Success"] = "Session created successfully";
                    return RedirectToAction(nameof(Index));
            }
        }

        private async Task LoadLookupsAsync(CancellationToken ct)
        {
            var lookups = await sessions.GetLookupsAsync(ct);
            ViewBag.Categories = new SelectList(lookups.Categories, nameof(LookupItem.Id), nameof(LookupItem.Name));
            ViewBag.Trainers = new SelectList(lookups.Trainers, nameof(LookupItem.Id), nameof(LookupItem.Name));
        }
    }
}
