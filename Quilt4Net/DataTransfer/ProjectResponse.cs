﻿namespace Tharga.Quilt4Net.DataTransfer
{
    public class ProjectResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Versions { get; set; }
        public int Sessions { get; set; }
        public int IssueTypes { get; set; }
        public int Issues { get; set; }
        public string DashboardColor { get; set; }
    }
}