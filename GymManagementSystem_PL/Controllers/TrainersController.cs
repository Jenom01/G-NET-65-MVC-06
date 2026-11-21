using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Trainers;
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

            switch (result)
            {
                case TrainerResult.EmailExists:
                    ModelState.AddModelError(nameof(model.Email), "This email is already used");
                    return View(model);

                case TrainerResult.PhoneExists:
                    ModelState.AddModelError(nameof(model.Phone), "This phone number is already used");
                    return View(model);

                default:
                    TempData["Success"] = "Trainer created successfully";
                    return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var trainer = await trainers.GetDetailsAsync(id, ct);

            if (trainer is null) return NotFound();

            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainer = await trainers.GetForUpdateAsync(id, ct);

            if (trainer is null) return NotFound();

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerEditViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await trainers.UpdateAsync(id, model, ct);

            switch (result)
            {
                case TrainerResult.NotFound:
                    return NotFound();

                case TrainerResult.EmailExists:
                    ModelState.AddModelError(nameof(model.Email), "This email is already used");
                    return View(model);

                case TrainerResult.PhoneExists:
                    ModelState.AddModelError(nameof(model.Phone), "This phone number is already used");
                    return View(model);

                default:
                    TempData["Success"] = "Trainer updated successfully";
                    return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await trainers.GetDetailsAsync(id, ct);

            if (trainer is null) return NotFound();

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await trainers.DeleteAsync(id, ct);

            switch (result)
            {
                case TrainerResult.NotFound:
                    return NotFound();

                case TrainerResult.HasSessions:
                    TempData["Error"] = "Cannot delete a trainer who has scheduled sessions";
                    break;

                case TrainerResult.Success:
                    TempData["Success"] = "Trainer deleted successfully";
                    break;

                default:
                    TempData["Error"] = "Something went wrong";
                    break;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
