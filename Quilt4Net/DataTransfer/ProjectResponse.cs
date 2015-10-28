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
        public IssueTypeResponse[] IssueTypes { get; set; }
        public IssueResponse[] Issues { get; set; }
        public SessionResponse[] Sessions { get; set; }
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

    public class IssueTypeResponse
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string ApplicationName { get; set; }
        public string VersionName { get; set; }
        public string Type { get; set; }
        public string ResponseMessage { get; set; }
        public int Ticket { get; set; }
        public string Level { get; set; }
        public IssueTypeResponse Inner { get; set; }
        public Guid SessionKey { get; set; }
    }

    public class IssueResponse
    {
        public string ApplicationName { get; set; }
        public string VersionName { get; set; }
        public DateTime IssueTime { get; set; }
        public IDictionary<string, string> Data { get; set; }
        public string UserInput { get; set; }
        public bool? Visible { get; set; }
        public string IssueTypeMessage { get; set; }
    }

    public class SessionResponse
    {
        public Guid SessionKey { get; set; }
        public string ApplicationName { get; set; }
        public string VersionName { get; set; }
    }
}