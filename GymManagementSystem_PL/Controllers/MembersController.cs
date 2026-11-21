using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Members;
using GymManagementSystem_PL.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    public class MembersController(IMemberService members) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await members.GetAllAsync(ct));

        [HttpGet]
        public IActionResult Create()
            => View(new MemberCreateViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberCreateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await members.CreateAsync(model, ct);

            if (result.IsFailure)
            {
                ModelState.AddError(result.Error);
                return View(model);
            }

            TempData["Success"] = "Member created successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await members.GetDetailsAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await members.GetHealthRecordAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await members.GetForUpdateAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, MemberEditViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await members.UpdateAsync(id, model, ct);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Member updated successfully";
                return RedirectToAction(nameof(Index));
            }

            if (result.Error.Type == ErrorType.NotFound) return NotFound();

            ModelState.AddError(result.Error);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await members.GetDetailsAsync(id, ct);

            return result.IsFailure ? NotFound() : View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await members.DeleteAsync(id, ct);

            if (result.IsFailure && result.Error.Type == ErrorType.NotFound) return NotFound();

            if (result.IsSuccess)
                TempData["Success"] = "Member deleted successfully";
            else
                TempData["Error"] = result.Error.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
