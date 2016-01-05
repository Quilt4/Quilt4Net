using System.Linq;
using System.Threading.Tasks;
using Quilt4Net.Core.Interfaces;
using Tharga.Toolkit.Console.Command.Base;

namespace Quilt4Net.Sample.Console.Commands.Session
{
    internal class ListSessionsCommand : ActionCommandBase
    {
        private readonly IQuilt4NetClient _client;

        public ListSessionsCommand(IQuilt4NetClient client)
            : base("List", "List sessions")
        {
            _client = client;
        }

        public override bool CanExecute()
        {
            return _client.Actions.User.IsAuthorized;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var sessions = (await _client.SessionHandler.GetListAsync()).ToArray();
            if (!sessions.Any()) return true;
            var title = new[] { new[] { "SessionKey", "Environment" } };
            var data = title.Union(sessions.Select(x => new[] { x.SessionKey.ToString(), x.Environment }).ToArray()).ToArray();
            OutputTable(data);
            return true;
        }
    }
}