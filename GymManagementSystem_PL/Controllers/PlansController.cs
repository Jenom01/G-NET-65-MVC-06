using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Plans;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    public class PlansController(IPlanService plans) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await plans.GetAllAsync(ct));

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await plans.GetDetailsAsync(id, ct);

            if (plan is null) return NotFound();

            return View(plan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await plans.GetForUpdateAsync(id, ct);

            if (plan is null) return NotFound();

            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, PlanEditViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await plans.UpdateAsync(id, model, ct);

            switch (result)
            {
                case PlanResult.NotFound:
                    return NotFound();

                case PlanResult.HasActiveMemberships:
                    ModelState.AddModelError(string.Empty, "Cannot update a plan that has active memberships");
                    return View(model);

                default:
                    TempData["Success"] = "Plan updated successfully";
                    return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await plans.ToggleStatusAsync(id, ct);

            switch (result)
            {
                case PlanResult.NotFound:
                    return NotFound();

                case PlanResult.HasActiveMemberships:
                    TempData["Error"] = "Cannot deactivate a plan that has active memberships";
                    break;

                default:
                    TempData["Success"] = "Plan Status Changed";
                    break;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
