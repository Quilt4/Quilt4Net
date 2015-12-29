using Quilt4Net.Core.Interfaces;
using Tharga.Toolkit.Console.Command.Base;

namespace Quilt4Net.Sample.Console.Commands.Issue.Type
{
    internal class IssueTypeCommands : ContainerCommandBase
    {
        public IssueTypeCommands(IClient client)
            : base("Type")
        {
            RegisterCommand(new ListIssueTypesCommand(client));
        }
    }
}