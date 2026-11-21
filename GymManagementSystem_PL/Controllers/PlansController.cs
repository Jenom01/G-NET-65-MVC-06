using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Plans;
using GymManagementSystem_PL.Extensions;
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
            var result = await plans.GetDetailsAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await plans.GetForUpdateAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, PlanEditViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await plans.UpdateAsync(id, model, ct);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Plan updated successfully";
                return RedirectToAction(nameof(Index));
            }

            if (result.Error.Type == ErrorType.NotFound) return NotFound();

            ModelState.AddError(result.Error);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await plans.ToggleStatusAsync(id, ct);

            if (result.IsFailure && result.Error.Type == ErrorType.NotFound) return NotFound();

            if (result.IsSuccess)
                TempData["Success"] = "Plan Status Changed";
            else
                TempData["Error"] = result.Error.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
