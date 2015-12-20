using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quilt4Net.Core.DataTransfer;
using Quilt4Net.Core.Events;

namespace Quilt4Net.Core.Interfaces
{
    public interface ISession
    {
        event EventHandler<SessionRegistrationStartedEventArgs> SessionRegistrationStartedEvent;
        event EventHandler<SessionRegistrationCompletedEventArgs> SessionRegistrationCompletedEvent;
        Task<SessionResponse> RegisterAsync();
        void RegisterStart();
        SessionResponse Register();
        bool IsRegistered { get; }
        Guid GetSessionKey();
        Task<IEnumerable<SessionRequest>> GetListAsync();
    }
}