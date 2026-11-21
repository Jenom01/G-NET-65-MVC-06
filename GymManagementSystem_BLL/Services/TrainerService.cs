using AutoMapper;
using GymManagementSystem_BLL.Errors;
using GymManagementSystem_BLL.Results;
using GymManagementSystem_BLL.ViewModels.Trainers;
using GymManagementSystem_DAL.Models;
using GymManagementSystem_DAL.Repositories;

namespace GymManagementSystem_BLL.Services
{
    public class TrainerService(IUnitOfWork uow, IMapper mapper) : ITrainerService
    {
        public async Task<IEnumerable<TrainerViewModel>> GetAllAsync(CancellationToken ct = default)
            => mapper.Map<IEnumerable<TrainerViewModel>>(await uow.Trainers.GetAllAsync(ct));

        public async Task<Result> CreateAsync(TrainerCreateViewModel model, CancellationToken ct = default)
        {
            model.Email = model.Email.Trim().ToLowerInvariant();
            model.Phone = model.Phone.Trim();

            if (await uow.Trainers.IsEmailTakenAsync(model.Email, ct: ct))
                return TrainerErrors.EmailExists;

            if (await uow.Trainers.IsPhoneTakenAsync(model.Phone, ct: ct))
                return TrainerErrors.PhoneExists;

            uow.Trainers.Add(mapper.Map<Trainer>(model));
            await uow.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result<TrainerDetailsViewModel>> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);

            if (trainer is null) return TrainerErrors.NotFound;

            return mapper.Map<TrainerDetailsViewModel>(trainer);
        }

        public async Task<Result<TrainerEditViewModel>> GetForUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);

            if (trainer is null) return TrainerErrors.NotFound;

            return mapper.Map<TrainerEditViewModel>(trainer);
        }

        public async Task<Result> UpdateAsync(int id, TrainerEditViewModel model, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);

            if (trainer is null) return TrainerErrors.NotFound;

            model.Email = model.Email.Trim().ToLowerInvariant();
            model.Phone = model.Phone.Trim();

            if (await uow.Trainers.IsEmailTakenAsync(model.Email, trainer.Id, ct))
                return TrainerErrors.EmailExists;

            if (await uow.Trainers.IsPhoneTakenAsync(model.Phone, trainer.Id, ct))
                return TrainerErrors.PhoneExists;

            mapper.Map(model, trainer);
            await uow.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
        {
            var trainer = await uow.Trainers.GetByIdAsync(id, ct);

            if (trainer is null) return TrainerErrors.NotFound;

            if (await uow.Trainers.HasActiveSessionsAsync(id, ct))
                return TrainerErrors.HasActiveSessions;

            uow.Trainers.Delete(trainer);
            await uow.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
