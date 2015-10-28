﻿using System;
using System.Collections.Generic;
using Tharga.Quilt4Net.Interfaces;

namespace Tharga.Quilt4Net.DataTransfer
{
    public class ProjectResponse
    {
        public string Name { get; set; }
        public IProjectInfo Info { get; set; }
        public ApplicationResponse[] Applications { get; set; }
        public VersionResponse[] Versions { get; set; }
    }

    public class ApplicationResponse
    {
        public string Name { get; set; }
    }

    public class VersionResponse
    {
        public string Name { get; set; }
        public string ApplicationName { get; set; }
        public DateTime? BuildTime { get; set; }
        public string SupportToolkit { get; set; }
    }
}