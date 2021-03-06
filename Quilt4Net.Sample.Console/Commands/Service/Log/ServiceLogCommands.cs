﻿using Quilt4Net.Core.Interfaces;
using Tharga.Toolkit.Console.Commands.Base;

namespace Quilt4Net.Sample.Console.Commands.Service.Log
{
    internal class ServiceLogCommands : ContainerCommandBase
    {
        public ServiceLogCommands(IQuilt4NetClient client)
            : base("Log")
        {
            RegisterCommand(new ListServiceLogCommand(client));
        }
    }
}
