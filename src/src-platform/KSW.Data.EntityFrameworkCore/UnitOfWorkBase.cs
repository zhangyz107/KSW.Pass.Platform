using KSW.Data;
using KSW.Data.Abstractions;
using KSW.Data.EntityFrameworkCore.ValueComparers;
using KSW.Data.EntityFrameworkCore.ValueConverters;
using KSW.Dates;
using KSW.Domain;
using KSW.Domain.Compare;
using KSW.Domain.Extending;
using KSW.Exceptions;
using KSW.Helpers;
using KSW.Properties;

namespace KSW.Data.EntityFrameworkCore;

/// <summary>
/// 工作单元基类
/// </summary>
public abstract class UnitOfWorkBase : DbContext, IUnitOfWork {

    #region 构造方法

    /// <summary>
    /// 初始化工作单元
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="options">配置</param>
    protected UnitOfWorkBase(DbContextOptions options )
        : base( options ) {

    }

    #endregion

    #region 属性
    /// <summary>
    /// 是否清除字符串两端的空白,默认为true
    /// </summary>
    protected virtual bool IsTrimString => true;

    #endregion

    #region OnConfiguring(配置)

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="optionsBuilder">配置生成器</param>
    protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder ) {
        //ConfigLog( optionsBuilder );
    }

    #endregion

    #region ConfigLog(配置日志)

    /// <summary>
    /// 配置日志
    /// </summary>
    /// <param name="optionsBuilder">配置生成器</param>
    //protected virtual void ConfigLog( DbContextOptionsBuilder optionsBuilder ) {
    //    if ( Environment == null )
    //        return;
    //    if ( Environment.IsProduction() )
    //        return;
    //    optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging();
    //}

    #endregion

    #region 配置连接字符串
    /// <summary>
    /// 配置租户连接字符串
    /// </summary>
    /// <param name="optionsBuilder">配置生成器</param>
    /// <param name="connectionString">连接字符串</param>
    protected virtual void ConfigTenantConnectionString(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {

    }

    #endregion

    #region OnModelCreating(配置模型)

    /// <summary>
    /// 配置模型
    /// </summary>
    /// <param name="modelBuilder">模型生成器</param>
    protected override void OnModelCreating( ModelBuilder modelBuilder ) {
        ApplyConfigurations( modelBuilder );
        foreach ( var entityType in modelBuilder.Model.GetEntityTypes() ) {
            //ApplyFilters( modelBuilder, entityType );
            ApplyExtraProperties( modelBuilder, entityType );
            ApplyVersion( modelBuilder, entityType );
            ApplyIsDeleted( modelBuilder, entityType );
            //ApplyTenantId( modelBuilder, entityType );
            ApplyUtc( modelBuilder, entityType );
            ApplyTrimString( modelBuilder, entityType );
        }
    }

    #endregion

    #region ApplyConfigurations(配置实体类型)

    /// <summary>
    /// 配置实体类型
    /// </summary>
    /// <param name="modelBuilder">模型生成器</param>
    protected virtual void ApplyConfigurations( ModelBuilder modelBuilder ) {
        modelBuilder.ApplyConfigurationsFromAssembly( GetType().Assembly );
    }

    #endregion

    #region ApplyExtraProperties(配置扩展属性)

    /// <summary>
    /// 配置扩展属性
    /// </summary>
    /// <param name="modelBuilder">模型生成器</param>
    /// <param name="entityType">实体类型</param>
    protected virtual void ApplyExtraProperties( ModelBuilder modelBuilder, IMutableEntityType entityType ) {
        if ( typeof( IExtraProperties ).IsAssignableFrom( entityType.ClrType ) == false )
            return;
        modelBuilder.Entity( entityType.ClrType )
            .Property( "ExtraProperties" )
            .HasColumnName( "ExtraProperties" )
            .HasComment( R.ExtraProperties )
            .HasConversion( new ExtraPropertiesValueConverter() )
            .Metadata.SetValueComparer( new ExtraPropertyDictionaryValueComparer() );
    }

    #endregion

    #region ApplyVersion(配置乐观锁)

    /// <summary>
    /// 配置乐观锁
    /// </summary>
    /// <param name="modelBuilder">模型生成器</param>
    /// <param name="entityType">实体类型</param>
    protected virtual void ApplyVersion( ModelBuilder modelBuilder, IMutableEntityType entityType ) {
        if ( typeof( IVersion ).IsAssignableFrom( entityType.ClrType ) == false )
            return;
        modelBuilder.Entity( entityType.ClrType )
            .Property( "Version" )
            .HasColumnName( "Version" )
            .HasComment( R.Version )
            .IsConcurrencyToken();
    }

    #endregion

    #region ApplyIsDeleted(配置逻辑删除)

    /// <summary>
    /// 配置逻辑删除
    /// </summary>
    /// <param name="modelBuilder">模型生成器</param>
    /// <param name="entityType">实体类型</param>
    protected virtual void ApplyIsDeleted( ModelBuilder modelBuilder, IMutableEntityType entityType ) {
        if ( typeof( IDelete ).IsAssignableFrom( entityType.ClrType ) == false )
            return;
        modelBuilder.Entity( entityType.ClrType )
            .Property( "IsDeleted" )
            .HasColumnName( "IsDeleted" )
            .HasComment( R.IsDeleted );
    }

    #endregion

    #region ApplyUtc(配置Utc日期)

    /// <summary>
    /// 配置Utc日期
    /// </summary>
    /// <param name="modelBuilder">模型生成器</param>
    /// <param name="entityType">实体类型</param>
    protected virtual void ApplyUtc( ModelBuilder modelBuilder, IMutableEntityType entityType ) {
        if ( TimeOptions.IsUseUtc == false )
            return;
        var properties = entityType.ClrType.GetProperties()
            .Where( property => ( property.PropertyType == typeof( DateTime ) || property.PropertyType == typeof( DateTime? ) ) && property.CanWrite &&
                                property.GetCustomAttribute<NotMappedAttribute>() == null )
            .ToList();
        properties.ForEach( property => {
            modelBuilder.Entity( entityType.ClrType )
                .Property( property.Name )
                .HasConversion( new DateTimeValueConverter() );
        } );
    }

    #endregion

    #region ApplyTrimString(配置清除空白字符串)

    /// <summary>
    /// 配置清除空白字符串
    /// </summary>
    /// <param name="modelBuilder">模型生成器</param>
    /// <param name="entityType">实体类型</param>
    protected virtual void ApplyTrimString( ModelBuilder modelBuilder, IMutableEntityType entityType ) {
        if ( IsTrimString == false )
            return;
        var properties = entityType.ClrType.GetProperties()
            .Where( property => property.PropertyType == typeof( string ) && property.CanWrite &&
                                property.GetCustomAttribute<NotMappedAttribute>() == null )
            .ToList();
        properties.ForEach( property => {
            modelBuilder.Entity( entityType.ClrType )
                .Property( property.Name )
                .HasConversion( new TrimStringValueConverter() );
        } );
    }

    #endregion

    #region CommitAsync(提交)

    /// <summary>
    /// 提交,返回影响的行数
    /// </summary>
    public async Task<int> CommitAsync() {
        try {
            return await SaveChangesAsync();
        }
        catch ( DbUpdateConcurrencyException ex ) {
            throw new ConcurrencyException( ex );
        }
    }

    #endregion

    #region SaveChangesAsync(保存)

    /// <summary>
    /// 保存
    /// </summary>
    public override async Task<int> SaveChangesAsync( CancellationToken cancellationToken = default ) {
        await SaveChangesBefore();
        var result = await base.SaveChangesAsync( cancellationToken );
        await SaveChangesAfter();
        return result;
    }

    #endregion

    #region SaveChangesBefore(保存前操作)

    /// <summary>
    /// 保存前操作
    /// </summary>
    protected virtual async Task SaveChangesBefore() {
        foreach ( var entry in ChangeTracker.Entries() ) {
            //AddDomainEvents( entry );
            switch ( entry.State ) {
                case EntityState.Added:
                    AddBefore( entry );
                    break;
                case EntityState.Modified:
                    UpdateBefore( entry );
                    break;
                case EntityState.Deleted:
                    DeleteBefore( entry );
                    break;
            }
        }
        //await PublishSaveBeforeEventsAsync();
    }

    #endregion

    #region AddBefore(添加前操作)

    /// <summary>
    /// 添加前操作
    /// </summary>
    protected virtual void AddBefore( EntityEntry entry ) {
        SetVersion( entry.Entity );
        //AddEntityCreatedEvent(entry.Entity);
    }

    #endregion

    #region UpdateBefore(修改前操作)

    /// <summary>
    /// 修改前操作
    /// </summary>
    protected virtual void UpdateBefore( EntityEntry entry ) {
        SetVersion( entry.Entity );
        //AddEntityUpdatedEvent(entry);
    }

    #endregion

    #region DeleteBefore(删除前操作)

    /// <summary>
    /// 删除前操作
    /// </summary>
    protected virtual void DeleteBefore( EntityEntry entry ) {
        //AddEntityDeletedEvent( entry.Entity );
    }

    #endregion

    #region SetVersion(设置版本号)

    /// <summary>
    /// 设置版本号
    /// </summary>
    protected virtual void SetVersion( object obj ) {
        if ( !( obj is IVersion entity ) )
            return;
        var version = GetVersion();
        if ( version == null )
            return;
        entity.Version = version;
    }

    #endregion

    #region GetVersion(获取版本号)

    /// <summary>
    /// 获取版本号
    /// </summary>
    protected virtual byte[] GetVersion() {
        return Encoding.UTF8.GetBytes( Guid.NewGuid().ToString() );
    }

    #endregion

    #region GetChangeValues(获取变更值集合)

    /// <summary>
    /// 获取变更值集合
    /// </summary>
    protected virtual ChangeValueCollection GetChangeValues( EntityEntry entry ) {
        var result = new ChangeValueCollection();
        var properties = entry.Metadata.GetProperties();
        foreach ( var property in properties ) {
            var propertyEntry = entry.Property( property.Name );
            if ( property.Name == "ExtraProperties" ) {
                var changeValues = GetExtraPropertiesChangeValues( property, propertyEntry );
                changeValues.ForEach( value => {
                    if ( value != null )
                        result.Add( value );
                } );
                continue;
            }
            var changeValue = GetPropertyChangeValue( property, propertyEntry );
            if ( changeValue == null )
                continue;
            result.Add( changeValue );
        }
        return result;
    }

    /// <summary>
    /// 获取扩展属性变更值集合
    /// </summary>
    protected virtual List<ChangeValue> GetExtraPropertiesChangeValues( IProperty property, PropertyEntry propertyEntry ) {
        var result = new List<ChangeValue>();
        if ( propertyEntry.CurrentValue is not ExtraPropertyDictionary currentExtraValue )
            return result;
        if ( propertyEntry.OriginalValue is not ExtraPropertyDictionary originalExtraValue )
            return result;
        foreach ( var key in currentExtraValue.Keys ) {
            currentExtraValue.TryGetValue( key, out var current );
            originalExtraValue.TryGetValue( key, out var original );
            var currentValue = KSW.Helpers.Json.ToJson( current );
            var originalValue = KSW.Helpers.Json.ToJson( original );
            result.Add( ToChangeValue( key, property.Name, originalValue, currentValue ) );
        }
        return result;
    }

    /// <summary>
    /// 转换为变更值
    /// </summary>
    protected virtual ChangeValue ToChangeValue( string name, string description, string originalValue, string currentValue ) {
        if ( originalValue == "[]" )
            originalValue = string.Empty;
        if ( currentValue == "[]" )
            currentValue = string.Empty;
        if ( originalValue == currentValue )
            return null;
        return new ChangeValue( name, description, originalValue, currentValue );
    }

    /// <summary>
    /// 获取属性变更值
    /// </summary>
    protected virtual ChangeValue GetPropertyChangeValue( IProperty property, PropertyEntry propertyEntry ) {
        var description = Reflection.GetDisplayNameOrDescription( property.PropertyInfo );
        return ToChangeValue( property.Name, description, propertyEntry.OriginalValue.SafeString(), propertyEntry.CurrentValue.SafeString() );
    }

    #endregion


    #region SaveChangesAfter(保存后操作)

    /// <summary>
    /// 保存后操作
    /// </summary>
    protected virtual async Task SaveChangesAfter() {
        //await PublishSaveAfterEventsAsync();
        //await ExecuteActionsAsync();
    }

    #endregion
}