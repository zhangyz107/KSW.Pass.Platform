using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.Abstractions;
using KSW.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 站点信息业务逻辑
    /// </summary>
    public class SiteInfoBLL : CrudServiceBase<SiteInfo>, ISiteInfoBLL
    {
        private readonly ISiteInfoRepository _repository;

        public SiteInfoBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ISiteInfoRepository repository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
        }

        public async Task<List<SiteInfoModel>> GetSiteInfosFromPinOverviewId(string id)
        {
            if (id.IsEmpty())
                return null;

            var result = await _repository.FindAllAsync(x => x.PinOverviewId.Equals(id.ToGuid()));
            var orderList = result?.OrderBy(x => x.SortId)?.ToList();

            return orderList?.MapToList<SiteInfoModel>();
        }

        public async Task CreateSiteByCountAsync(string id, int? count)
        {
            if (count == null || count == 0)
                return;

            var list = await _repository.FindAllAsync(x => x.PinOverviewId.Equals(id.ToGuid()));
            if (list.Count > count)
            {
                var removeList = list.Skip(count ?? 0).ToList();
                await _repository.RemoveAsync(removeList);
                await CommitAsync();
            }
            else if (list.Count < count)
            {
                var addList = new List<SiteInfo>();
                var addCount = count - list.Count;
                for (int i = 0; i < addCount; i++)
                {
                    var entity = new SiteInfo();
                    entity.Init();
                    entity.PinOverviewId = id.ToGuid();
                    entity.SortId = list.Count + i;
                    entity.SiteName = $"Site {entity.SortId}";
                    addList.Add(entity);
                }
                await _repository.AddAsync(addList);
                await CommitAsync();
            }
        }
    }
}
