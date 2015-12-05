using System.Linq;
using System.Threading.Tasks;
using Tharga.Quilt4Net;
using Tharga.Toolkit.Console.Command.Base;

namespace Quilt4Net.Sample.Console.Commands.Project
{
    internal class ListProjectsCommand : ActionCommandBase
    {
        private readonly Client _client;

        public ListProjectsCommand(Client client)
            : base("List", "List projects")
        {
            _client = client;
        }

        public override bool CanExecute()
        {
            return _client.User.IsAuthorized;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var projects = (await _client.Project.GetListAsync()).ToArray();
            if (!projects.Any()) return true;
            var title = new[] { new[] { "Name", "ProjectApiKey" } };
            var data = title.Union(projects.Select(x => new[] { x.Name, x.ProjectApiKey }).ToArray()).ToArray();
            OutputTable(data);
            return true;
        }
    }
}