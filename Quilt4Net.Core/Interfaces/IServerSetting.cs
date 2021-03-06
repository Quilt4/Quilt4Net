using System.Collections.Generic;
using System.Threading.Tasks;
using Quilt4Net.Core.DataTransfer;

namespace Quilt4Net.Core.Interfaces
{
    public interface IServerSetting
    {
        Task<IEnumerable<SettingResponse>> GetListAsync();
    }
}