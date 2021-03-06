﻿using System.Reflection;
using System.Threading.Tasks;
using Quilt4Net.Core.DataTransfer;

namespace Quilt4Net.Interfaces
{
    public interface ISessionHandler : Core.Interfaces.ISessionHandler
    {
        Task<SessionResult> RegisterAsync(Assembly firstAssembly);
        void RegisterStart(Assembly firstAssembly);
        SessionResult Register(Assembly firstAssembly);
        void SetFirstAssembly(Assembly firstAssembly);
    }
}
