using System;

namespace Quilt4Net.Core.Interfaces
{
    public interface IClient : IDisposable
    {
        IIssue Issue { get; }
        ISession Session { get; }
        IUser User { get; }
        IProject Project { get; }
    }
}