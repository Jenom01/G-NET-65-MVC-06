using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Trainers;
using GymManagementSystem_PL.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    public class TrainersController(ITrainerService trainers) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await trainers.GetAllAsync(ct));

        [HttpGet]
        public IActionResult Create()
            => View(new TrainerCreateViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerCreateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await trainers.CreateAsync(model, ct);

            if (result.IsFailure)
            {
                ModelState.AddError(result.Error);
                return View(model);
            }

            TempData["Success"] = "Trainer created successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await trainers.GetDetailsAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await trainers.GetForUpdateAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, TrainerEditViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await trainers.UpdateAsync(id, model, ct);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Trainer updated successfully";
                return RedirectToAction(nameof(Index));
            }

            if (result.Error.Type == ErrorType.NotFound) return NotFound();

            ModelState.AddError(result.Error);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await trainers.GetDetailsAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await trainers.DeleteAsync(id, ct);

            if (result.IsFailure && result.Error.Type == ErrorType.NotFound) return NotFound();

            if (result.IsSuccess)
                TempData["Success"] = "Trainer deleted successfully";
            else
                TempData["Error"] = result.Error.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
