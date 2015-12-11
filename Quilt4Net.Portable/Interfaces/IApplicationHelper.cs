using System.Reflection;
using Quilt4Net.Core.DataTransfer;

namespace Quilt4Net.Core.Interfaces
{
    public interface IApplicationHelper
    {
        ApplicationData GetApplicationData(string projectApiKey, Assembly firstAssembly);
    }
}