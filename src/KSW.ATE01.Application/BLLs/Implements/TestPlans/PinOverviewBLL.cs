using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    public class PinOverviewBLL : CrudServiceBase<PinOverview>, IPinOverviewBLL
    {
        private readonly IPinOverviewRepository _repository;

        public PinOverviewBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IPinOverviewRepository repository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
        }

        public async Task<PinOverviewModel> GetByIdAsync(string id)
        {
            if (id.IsEmpty())
                return null;

            var result = await _repository?.FindByIdAsync(id);
            return result?.MapTo<PinOverviewModel>();
        }

        public async Task<PinOverviewModel> GetPinOverviewFromProjectIdAsync(string id)
        {
            if (id.IsEmpty())
                return null;

            var result = await _repository?.FindAllAsync(x => x.ProjectInfoId.Equals(id.ToGuid()));
            return result?.FirstOrDefault()?.MapTo<PinOverviewModel>();
        }

        public async Task<bool> SaveAsync(PinOverviewModel model)
        {
            var result = false;
            try
            {
                var entity = model.MapTo<PinOverview>();
                if (model.Id.IsEmpty())
                {
                    entity.Init();
                    await CreateAsync(entity);
                }
                else
                    await UpdateAsync(model.Id, entity);
                result = true;

            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
    }
}
