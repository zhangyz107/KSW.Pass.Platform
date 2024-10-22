using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Helpers
{
    /// <summary>
    /// 容器操作
    /// </summary>
    public static class Ioc
    {
        /// <summary>
        /// 容器
        /// </summary>
        private static readonly IContainerExtension _container = new DryIocContainerExtension(new DryIoc.Container(DryIocContainerExtension.DefaultRules));

        /// <summary>
        /// 获取服务提供器操作
        /// </summary>
        private static Func<IContainerProvider> _getServiceProviderAction;

        /// <summary>
        /// 获取服务集合
        /// </summary>
        public static IContainerExtension GetcContainerExtension()
        {
            return _container;
        }

        /// <summary>
        /// 获取
        /// </summary>
        public static IContainerProvider GetContainerProvider()
        {
            var provider = _getServiceProviderAction?.Invoke();
            if (provider != null)
                return provider;
            return _container;
        }

        /// <summary>
        /// 设置获取服务提供器操作
        /// </summary>
        /// <param name="action">获取服务提供器操作</param>
        public static void SetServiceProviderAction(Func<IContainerProvider> action)
        {
            _getServiceProviderAction = action;
        }
    }
}
