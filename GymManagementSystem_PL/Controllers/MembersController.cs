using GymManagementSystem_BLL.Services;
using GymManagementSystem_BLL.ViewModels.Members;
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

            switch (result)
            {
                case MemberResult.EmailExists:
                    ModelState.AddModelError(nameof(model.Email), "This email is already used");
                    return View(model);

                case MemberResult.PhoneExists:
                    ModelState.AddModelError(nameof(model.Phone), "This phone number is already used");
                    return View(model);

                default:
                    TempData["Success"] = "Member created successfully";
                    return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var member = await members.GetDetailsAsync(id, ct);

            if (member is null) return NotFound();

            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await members.GetHealthRecordAsync(id, ct);

            if (healthRecord is null) return NotFound();

            return View(healthRecord);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var member = await members.GetForUpdateAsync(id, ct);

            if (member is null) return NotFound();

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, MemberEditViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await members.UpdateAsync(id, model, ct);

            switch (result)
            {
                case MemberResult.EmailExists:
                    ModelState.AddModelError(nameof(model.Email), "This email is already used");
                    return View(model);

                case MemberResult.PhoneExists:
                    ModelState.AddModelError(nameof(model.Phone), "This phone number is already used");
                    return View(model);

                case MemberResult.NotFound:
                    return NotFound();

                default:
                    TempData["Success"] = "Member updated successfully";
                    return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await members.GetDetailsAsync(id, ct);

            if (member is null) return NotFound();

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await members.DeleteAsync(id, ct);

            switch (result)
            {
                case MemberResult.NotFound:
                    return NotFound();

                case MemberResult.HasActiveBookings:
                    TempData["Error"] = "Cannot delete a member with active bookings";
                    break;

                case MemberResult.Success:
                    TempData["Success"] = "Member deleted successfully";
                    break;

                default:
                    TempData["Error"] = "Something went wrong";
                    break;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
