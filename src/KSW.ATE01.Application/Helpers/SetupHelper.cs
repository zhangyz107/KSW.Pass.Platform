using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.Win32;

namespace KSW.ATE01.Application.Helpers
{
    /// <summary>
    /// 已安装应用程序帮助类
    /// </summary>
    public static class SetupHelper
    {
        public static ISetupInstance GetSetupInstance(bool isPreRelease)
        {
            return GetSetupInstances().First(i => IsPreRelease(i) == isPreRelease);
        }

        public sealed record VsInstance(string DisplayName, Version Version, string InstallPath);

        public static (IReadOnlyList<VsInstance> all, string? latestPath) GetAllAndLatestPath()
        {
            var list = new List<VsInstance>();

            // 1) 先尝试 VSSetup（精准覆盖 VS 2017+ 多实例）
            try
            {
                foreach (var inst in EnumerateViaVsSetup())
                    list.Add(inst);
            }
            catch
            {
                // 忽略：有些环境上未注册 VS Installer COM
            }

            // 2) 回退/补充：注册表（覆盖 VS 2010–2022）
            foreach (var inst in EnumerateViaRegistry())
                list.Add(inst);

            // 去重：按安装路径去重
            list = list
                .GroupBy(x => NormalizePath(x.InstallPath), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            // 选最新：按 Version 降序
            var latest = list
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            return (list, latest?.InstallPath);
        }

        private static IEnumerable<VsInstance> EnumerateViaVsSetup()
        {
            // 需要 Microsoft.VisualStudio.Setup.Configuration.Interop
            var setup = (ISetupConfiguration2)new SetupConfiguration();
            var e = setup.EnumAllInstances();
            var fetched = 0;
            var arr = new ISetupInstance2[1];

            do
            {
                e.Next(1, arr, out fetched);
                if (fetched == 0) break;

                var s = arr[0];
                // 仅包含已完整安装/可用的实例
                var state = s.GetState();
                if ((state & InstanceState.Local) == 0) continue;

                var versionStr = s.GetInstallationVersion(); // e.g. "17.11.35312.102"
                if (!Version.TryParse(NormalizeVersion(versionStr), out var ver)) continue;

                var path = s.GetInstallationPath();
                if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) continue;

                var disp = Safe(() => s.GetDisplayName()) ?? $"Visual Studio {ver}";
                yield return new VsInstance(disp, ver, path);
            } while (fetched > 0);
        }

        private static IEnumerable<VsInstance> EnumerateViaRegistry()
        {
            // 支持 HKLM\...\Microsoft\VisualStudio\{Version}\InstallDir
            // 和 2017+ 的 Installer 配置
            var hives = new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser };
            var views = new[] { RegistryView.Registry64, RegistryView.Registry32 };

            foreach (var hive in hives)
                foreach (var view in views)
                {
                    try
                    {
                        using var baseKey = RegistryKey.OpenBaseKey(hive, view);

                        // 旧版本（2010–2015）
                        using (var vsKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio"))
                        {
                            if (vsKey != null)
                            {
                                foreach (var sub in vsKey.GetSubKeyNames())
                                {
                                    // 过滤类似 "10.0", "11.0", "12.0", "14.0", "15.0"
                                    if (!char.IsDigit(sub.FirstOrDefault())) continue;

                                    using var subKey = vsKey.OpenSubKey(sub);
                                    var installDir = subKey?.GetValue("InstallDir") as string;
                                    var path = string.IsNullOrWhiteSpace(installDir)
                                        ? subKey?.GetValue("InstallLocation") as string
                                        : installDir;

                                    if (string.IsNullOrWhiteSpace(path)) continue;

                                    // 版本字符串规范化
                                    if (!Version.TryParse(NormalizeVersion(sub), out var ver)) continue;

                                    var root = Path.GetFullPath(path);
                                    var display = $"Visual Studio {ver}";
                                    yield return new VsInstance(display, ver, root);
                                }
                            }
                        }

                        // 2017+（从 Installer 记录补充）
                        using (var setupKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\Setup\Instances"))
                        {
                            if (setupKey != null)
                            {
                                foreach (var instName in setupKey.GetSubKeyNames())
                                {
                                    using var instKey = setupKey.OpenSubKey(instName);
                                    var path = instKey?.GetValue("InstallationPath") as string;
                                    var verStr = instKey?.GetValue("InstallationVersion") as string;

                                    if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(verStr)) continue;
                                    if (!Directory.Exists(path)) continue;

                                    if (!Version.TryParse(NormalizeVersion(verStr), out var ver)) continue;

                                    var display = instKey?.GetValue("DisplayName") as string ?? $"Visual Studio {ver}";
                                    yield return new VsInstance(display, ver, Path.GetFullPath(path));
                                }
                            }
                        }
                    }
                    finally
                    {

                    }
                }
        }

        private static string NormalizeVersion(string v)
        {
            // Version.Parse 需要最多四段；补齐/截断到 4 段
            var parts = v.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                         .Take(4).ToList();
            while (parts.Count < 4) parts.Add("0");
            return string.Join('.', parts);
        }

        private static string NormalizePath(string p)
        {
            try { return Path.GetFullPath(p.Trim().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)); }
            catch { return p ?? string.Empty; }
        }

        private static string? Safe(Func<string> f)
        {
            try { return f(); } catch { return null; }
        }

        public static IEnumerable<ISetupInstance> GetSetupInstances()
        {
            ISetupConfiguration setupConfiguration = new SetupConfiguration();
            IEnumSetupInstances enumerator = setupConfiguration.EnumInstances();

            int count;
            do
            {
                ISetupInstance[] setupInstances = new ISetupInstance[1];
                enumerator.Next(1, setupInstances, out count);
                if (count == 1 && setupInstances[0] != null)
                {
                    yield return setupInstances[0];
                }
            }
            while (count == 1);
        }

        private static bool IsPreRelease(ISetupInstance setupInstance)
        {
            ISetupInstanceCatalog setupInstanceCatalog = (ISetupInstanceCatalog)setupInstance;
            return setupInstanceCatalog.IsPrerelease();
        }
    }
}
