using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Quilt4Net.DataTransfer;
using Tharga.Quilt4Net.Interfaces;

namespace Tharga.Quilt4Net
{
    public class Project
    {
        private readonly IWebApiClient _webApiClient;

        internal Project(IWebApiClient webApiClient)
        {
            _webApiClient = webApiClient;
        }

        public async Task<CreateProjectResponse> CreateAsync(string projectName, string dashboardColor = null)
        {
            return await _webApiClient.ExecuteQueryAsync<CreateProjectRequest, CreateProjectResponse>("Project", "Create", new CreateProjectRequest { ProjectKey = Guid.NewGuid(), Name = projectName, DashboardColor = dashboardColor });
        }

        public async Task<IEnumerable<ProjectResponse>> GetAllAsync()
        {
            var result = await _webApiClient.ExecuteGet<ProjectResponse>("Project", "List");
            return result;
        }

        public async Task UpdateAsync(Guid projectKey, string projectName, string dashboardColor)
        {
            await _webApiClient.ExecuteCommandAsync("Project", "Update", new UpdateProjectRequest { ProjectKey = projectKey, Name = projectName, DashboardColor = dashboardColor });
        }
    }
}