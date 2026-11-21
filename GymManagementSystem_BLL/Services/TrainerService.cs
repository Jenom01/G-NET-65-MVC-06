using AutoMapper;
using GymManagementSystem_BLL.ViewModels.Trainers;
using GymManagementSystem_DAL.Models;
using GymManagementSystem_DAL.Repositories;

namespace GymManagementSystem_BLL.Services
{
    public class TrainerService(IUnitOfWork uow, IMapper mapper) : ITrainerService
    {
        public async Task<IEnumerable<TrainerViewModel>> GetAllAsync(CancellationToken ct = default)
            => mapper.Map<IEnumerable<TrainerViewModel>>(await uow.Trainers.GetAllAsync(ct));

        public async Task<TrainerResult> CreateAsync(TrainerCreateViewModel model, CancellationToken ct = default)
        {
            model.Email = model.Email.Trim().ToLowerInvariant();
            model.Phone = model.Phone.Trim();

            if (await uow.Trainers.IsEmailTakenAsync(model.Email, ct: ct))
                return TrainerResult.EmailExists;

            if (await uow.Trainers.IsPhoneTakenAsync(model.Phone, ct: ct))
                return TrainerResult.PhoneExists;

            uow.Trainers.Add(mapper.Map<Trainer>(model));
            await uow.SaveChangesAsync(ct);

            return TrainerResult.Success;
        }

        public async Task<TrainerDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);
            return trainer is null ? null : mapper.Map<TrainerDetailsViewModel>(trainer);
        }

        public async Task<TrainerEditViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);
            return trainer is null ? null : mapper.Map<TrainerEditViewModel>(trainer);
        }

        public async Task<TrainerResult> UpdateAsync(int id, TrainerEditViewModel model, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);

            if (trainer is null) return TrainerResult.NotFound;

            model.Email = model.Email.Trim().ToLowerInvariant();
            model.Phone = model.Phone.Trim();

            if (await uow.Trainers.IsEmailTakenAsync(model.Email, trainer.Id, ct))
                return TrainerResult.EmailExists;
            if (await uow.Trainers.IsPhoneTakenAsync(model.Phone, trainer.Id, ct))
                return TrainerResult.PhoneExists;

            mapper.Map(model, trainer);
            await uow.SaveChangesAsync(ct);

            return TrainerResult.Success;
        }

        public async Task<TrainerResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);

            if (trainer is null) return TrainerResult.NotFound;

            if (await uow.Trainers.HasActiveSessionsAsync(id, ct))
                return TrainerResult.HasSessions;

            uow.Trainers.Delete(trainer);
            await uow.SaveChangesAsync(ct);

            return TrainerResult.Success;
        }
    }
}
