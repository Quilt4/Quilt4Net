﻿using System;
using Quilt4Net.Core;
using Quilt4Net.Core.Interfaces;

namespace Quilt4Net
{
    public class Client : IClient
    {
        private static readonly object _syncRoot = new object();
        private static IClient _instance;
        private static bool _instanceCreated = false;

        private readonly IConfiguration _configuration;
        private readonly IWebApiClient _webApiClient;
        private readonly Lazy<IUser> _user;
        private readonly Lazy<IProject> _project;
        private readonly Lazy<ISession> _session;
        private readonly Lazy<IIssue> _issue;

        public Client(IConfiguration configuration)
        {
            lock (_syncRoot)
            {
                if (_instance != null) throw new InvalidOperationException("The client has been activated in singleton mode. Do not use 'Client.Instance' if you want to create your own instances of the client object.");

                _configuration = configuration;
                _webApiClient = new WebApiClient(_configuration);
                _user = new Lazy<IUser>(() => new User(_webApiClient));
                _project = new Lazy<IProject>(() => new Project(_webApiClient));
                _session = new Lazy<ISession>(() => new Session(_webApiClient, _configuration, new ApplicationHelper(_configuration), new MachineHelper(), new UserHelper()));
                _issue = new Lazy<IIssue>(() => new Issue(_session, _webApiClient, _configuration));

                _instanceCreated = true;
            }
        }

        public static IClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            if (_instanceCreated) throw new InvalidOperationException("The client has been activated in instance mode. Do not use 'new Client(???)' if you want to use the singleton instance of the client object.");

                            _instance = new Client(Quilt4Net.Configuration.Instance);
                        }
                    }
                }

                return _instance;
            }
        }

        public IConfiguration Configuration => _configuration;
        public IWebApiClient WebApiClient => _webApiClient;
        public IUser User => _user.Value;
        public IProject Project => _project.Value;
        public ISession Session => _session.Value;
        public IIssue Issue => _issue.Value;

        public void Dispose()
        {
            if (_session.IsValueCreated)
            {
                _session.Value.End();
            }
        }
    }
}