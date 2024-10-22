using KSW.Data.Abstractions;
using KSW.Domain.Compare;
using KSW.Domain.Entities;
using KSW.Domain.Repositories;
using KSW.Properties;
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

        #region CreateAsync(创建)

        /// <summary>
        /// 创建实体
        /// </summary>
        public virtual async Task CreateAsync(TEntity entity)
        {
            await CreateBeforeAsync(entity);
            entity.Init();
            await _repository.AddAsync(entity);
            await CreateAfterAsync(entity);
            await CommitAsync();
            await CreateCommitAfterAsync(entity);
        }

        /// <summary>
        /// 创建前操作
        /// </summary>
        /// <param name="entity">实体</param>
        protected virtual Task CreateBeforeAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 创建后操作
        /// </summary>
        /// <param name="entity">实体</param>
        protected virtual Task CreateAfterAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 提交工作单元
        /// </summary>
        protected virtual async Task CommitAsync()
        {
            await UnitOfWork.CommitAsync();
        }

        /// <summary>
        /// 创建提交后操作
        /// </summary>
        /// <param name="entity">实体</param>
        protected virtual Task CreateCommitAfterAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region UpdateAsync(修改)

        /// <summary>
        /// 查找旧实体
        /// </summary>
        /// <param name="id">标识</param>
        private async Task<TEntity> FindOldEntityAsync(object id)
        {
            return await _repository.FindByIdAsync(id);
        }

        public virtual async Task UpdateAsync(string id, TEntity entity)
        {
            var oldEntity = await FindOldEntityAsync(id);
            oldEntity.CheckNull(nameof(oldEntity));
            entity.CheckNull(nameof(entity));
            var changes = oldEntity.GetChanges(entity);
            await UpdateBeforeAsync(entity);
            await _repository.UpdateAsync(entity);
            await UpdateAfterAsync(entity);
            await CommitAsync();
            await UpdateCommitAfterAsync(entity, changes);
        }

        /// <summary>
        /// 修改前操作
        /// </summary>
        /// <param name="entity">实体</param>
        protected virtual Task UpdateBeforeAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 修改后操作
        /// </summary>
        /// <param name="entity">实体</param>
        protected virtual Task UpdateAfterAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 修改提交后操作
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="changeValues">变更值集合</param>
        protected virtual Task UpdateCommitAfterAsync(TEntity entity, ChangeValueCollection changeValues)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
