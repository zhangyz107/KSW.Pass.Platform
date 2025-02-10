using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Reflections
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly string _pluginAssemblyDir;
        private List<Assembly> _loadedAssemblies;
        public PluginLoadContext(string pluginAssemblyDir, Assembly[] loadedAssemblies) : base(isCollectible: true)
        {
            _pluginAssemblyDir = pluginAssemblyDir;
            _loadedAssemblies = loadedAssemblies.IsEmpty() ? null : new List<Assembly>(loadedAssemblies);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // 已加载项不再加载
            if (!_loadedAssemblies.IsEmpty())
            {
                var assembly = _loadedAssemblies.FirstOrDefault(x => x.FullName.Equals(assemblyName.FullName));
                if (assembly != null)
                    return assembly;
            }

            // 在这里查找依赖的程序集并加载
            string assemblyPath = Path.Combine(AppContext.BaseDirectory, assemblyName.Name + ".dll");
            if (File.Exists(assemblyPath))
                return LoadFromAssemblyPath(assemblyPath);

            assemblyPath = Path.Combine(_pluginAssemblyDir, assemblyName.Name + ".dll");
            if (File.Exists(assemblyPath))
                return LoadFromAssemblyPath(assemblyPath);

            return null; // 返回null表示未能加载程序集
        }
    }
}
