using System.Threading.Tasks;

namespace KSW.ATE01.Extension.Shared.Integration
{
    internal interface ISwitchableFeature
    {
        Task SwitchAsync(bool on);
    }
}
