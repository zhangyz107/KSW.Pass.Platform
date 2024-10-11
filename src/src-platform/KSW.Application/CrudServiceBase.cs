using KSW.Data.Abstractions;
using KSW.Domain.Entities;
using KSW.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Application
{
    /// <summary>
    /// 增删改查服务
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public abstract class CrudServiceBase<TEntity> : CrudServiceBase<TEntity, Guid>
        where TEntity : class, IAggregateRoot<TEntity, Guid>, new()
    {
        /// <summary>
        /// 初始化增删改查服务
        /// </summary>
        /// <param name="containerProvider">容器提供器</param>
        /// <param name="unitOfWork">工作单元</param>
        /// <param name="repository">仓储</param>
        protected CrudServiceBase(IContainerProvider containerProvider, IUnitOfWork unitOfWork, IRepository<TEntity, Guid> repository) : base(containerProvider, unitOfWork, repository)
        {

        }
    }


    /// <summary>
    /// 增删改查服务
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">实体标识类型</typeparam>
    public abstract class CrudServiceBase<TEntity, TKey> : ServiceBase 
        where TEntity : class, IAggregateRoot<TEntity, TKey>, new()
    {
        #region 字段

        /// <summary>
        /// 仓储
        /// </summary>
        private readonly IRepository<TEntity, TKey> _repository;

        #endregion

        /// <summary>
        /// 初始化增删改查服务
        /// </summary>
        /// <param name="containerProvider">容器提供器</param>
        /// <param name="unitOfWork">工作单元</param>
        /// <param name="repository">仓储</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected CrudServiceBase(IContainerProvider containerProvider, IUnitOfWork unitOfWork, IRepository<TEntity, TKey> repository) : base(containerProvider)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region 属性

        /// <summary>
        /// 工作单元
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }

        #endregion
    }
}
