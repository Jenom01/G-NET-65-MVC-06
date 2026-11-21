using AutoMapper;
using GymManagementSystem_BLL.ViewModels.HealthRecords;
using GymManagementSystem_BLL.ViewModels.Members;
using GymManagementSystem_DAL.Models;
using GymManagementSystem_DAL.Repositories;

namespace GymManagementSystem_BLL.Services
{
    public class MemberService(IUnitOfWork uow, IMapper mapper) : IMemberService
    {
        public async Task<IEnumerable<MemberIndexViewModel>> GetAllAsync(CancellationToken ct = default)
            => mapper.Map<IEnumerable<MemberIndexViewModel>>(await uow.Members.GetAllAsync(ct));

        public async Task<MemberResult> CreateAsync(MemberCreateViewModel model, CancellationToken ct)
        {
            model.Email = model.Email.Trim().ToLowerInvariant();
            model.Phone = model.Phone.Trim();

            if (await uow.Members.IsEmailTakenAsync(model.Email, ct: ct))
                return MemberResult.EmailExists;

            if (await uow.Members.IsPhoneTakenAsync(model.Phone, ct: ct))
                return MemberResult.PhoneExists;

            uow.Members.Add(mapper.Map<Member>(model));
            await uow.SaveChangesAsync(ct);

            return MemberResult.Success;
        }

        public async Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var member = await uow.Members.GetDetailsAsync(id, ct);

            if (member is null) return null;

            var now = DateTime.UtcNow;

            var activeMembership = member.Memberships
                .Where(ms => ms.StartDate <= now && ms.EndDate > now)
                .OrderByDescending(ms => ms.EndDate)
                .FirstOrDefault();

            return new MemberDetailsViewModel
            {
                Id = id,
                Name = member.Name.Trim(),
                PhotoUrl = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",

                PlanName = activeMembership?.Plan.Name ?? "No Active Plan",
                MembershipStartDate = activeMembership?.StartDate.ToShortDateString() ?? "N/A",
                MembershipEndDate = activeMembership?.EndDate.ToShortDateString() ?? "N/A"
            };
        }

        public async Task<HealthRecordDetailsViewModel?> GetHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await uow.HealthRecords.GetWhereAsync(hr => hr.MemberId == memberId, ct);
            return healthRecord is null ? null : mapper.Map<HealthRecordDetailsViewModel>(healthRecord);
        }

        public async Task<MemberEditViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await uow.Members.GetByIdAsync(id, ct);
            return member is null ? null : mapper.Map<MemberEditViewModel>(member);
        }

        public async Task<MemberResult> UpdateAsync(int id, MemberEditViewModel model, CancellationToken ct = default)
        {
            var member = await uow.Members.GetByIdAsync(id, ct);

            if (member is null) return MemberResult.NotFound;

            model.Email = model.Email.Trim().ToLowerInvariant();
            model.Phone = model.Phone.Trim();

            if (await uow.Members.IsEmailTakenAsync(model.Email, member.Id, ct))
                return MemberResult.EmailExists;

            if (await uow.Members.IsPhoneTakenAsync(model.Phone, member.Id, ct))
                return MemberResult.PhoneExists;

            mapper.Map(model, member);
            await uow.SaveChangesAsync(ct);

            return MemberResult.Success;
        }

        public async Task<MemberResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            var member = await uow.Members.GetByIdAsync(id, ct, m => m.HealthRecord);

            if (member is null) return MemberResult.NotFound;

            if (await uow.Members.HasActiveBookingsAsync(id, ct))
                return MemberResult.HasActiveBookings;

            if (member.HealthRecord is not null)
                uow.HealthRecords.Delete(member.HealthRecord);

            uow.Members.Delete(member);
            await uow.SaveChangesAsync(ct);

            return MemberResult.Success;
        }
    }
}
