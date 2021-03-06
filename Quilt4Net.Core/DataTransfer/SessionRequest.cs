﻿using System;

namespace Quilt4Net.Core.DataTransfer
{
    public class SessionRequest
    {
        internal SessionRequest()
        {
        }

        public string ProjectApiKey { get; set; }
        public DateTime ClientStartTime { get; set; }
        public string Environment { get; set; }
        public ApplicationData Application { get; set; }
        public MachineData Machine { get; set; }
        public UserData User { get; set; }
    }
}