﻿using Quilt4Net.Core.Interfaces;
using Quilt4Net.Sample.Console.Commands.Issue.Type;
using Tharga.Toolkit.Console.Command.Base;

namespace Quilt4Net.Sample.Console.Commands.Issue
{
    internal class IssueCommands : ContainerCommandBase
    {
        public IssueCommands(IClient client)
            : base("Issue")
        {
            RegisterCommand(new RegisterIssueCommand(client));
            RegisterCommand(new IssueTypeCommands(client));
        }
    }
}