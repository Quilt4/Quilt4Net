﻿using System;

namespace Tharga.Quilt4Net.DataTransfer
{
    public class RegisterSessionRequest
    {
        public Guid SessionKey { get; set; }
        public string ProjectApiKey { get; set; }
        public DateTime ClientStartTime { get; set; }
        public string Environment { get; set; }
        public ApplicationData Application { get; set; }
        public MachineData Machine { get; set; }
        public UserData User { get; set; }
    }
}