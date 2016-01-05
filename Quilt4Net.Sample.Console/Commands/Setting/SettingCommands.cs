﻿using Quilt4Net.Core.Interfaces;
using Tharga.Toolkit.Console.Command.Base;

namespace Quilt4Net.Sample.Console.Commands.Setting
{
    internal class SettingCommands : ContainerCommandBase
    {
        public SettingCommands(IClient client)
            : base("Setting")
        {
            RegisterCommand(new ListSettingsCommand(client));
        }
    }
}