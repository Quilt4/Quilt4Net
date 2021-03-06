﻿using Quilt4Net.Core.Interfaces;

namespace Quilt4Net.Core
{
    public abstract class ConfigurationBase : IConfiguration
    {
        internal static readonly object SyncRoot = new object();
        protected string _projectApiKey;
        protected bool? _enabled;
        protected bool? _useBuildTime;
        protected string _applicationName;
        protected string _applicationVersion;
        protected bool? _allowMultipleInstances;

        public virtual bool Enabled
        {
            get
            {
                if (_enabled != null) return _enabled.Value;

                lock (SyncRoot)
                {
                    if (_enabled == null)
                    {
                        _enabled = true;
                    }
                }

                return _enabled.Value;
            }

            set
            {
                _enabled = value;
            }
        }

        public virtual string ProjectApiKey
        {
            get
            {
                if (_projectApiKey != null) return _projectApiKey;

                lock (SyncRoot)
                {
                    if (_projectApiKey == null)
                    {
                        throw new ExpectedIssues(this).GetException(ExpectedIssues.ProjectApiKeyNotSet);
                    }
                }

                return _projectApiKey;
            }

            set
            {
                if (value == null)
                {
                    throw new ExpectedIssues(this).GetException(ExpectedIssues.CannotSetProjectApiKey);
                }

                _projectApiKey = value;
            }
        }

        public virtual bool UseBuildTime
        {
            get
            {
                if (_useBuildTime != null) return _useBuildTime.Value;

                //If there is no setting, read from config file to populate the value
                lock (SyncRoot)
                {
                    if (_useBuildTime == null)
                    {
                        _useBuildTime = false;
                    }
                }

                return _useBuildTime.Value;
            }

            set
            {
                _useBuildTime = value;
            }
        }

        public virtual string ApplicationName
        {
            get
            {
                return _applicationName;
            }

            set
            {
                _applicationName = value;
            }
        }

        public virtual string ApplicationVersion
        {
            get
            {
                return _applicationVersion;
            }

            set
            {
                _applicationVersion = value;
            }
        }

        public virtual bool AllowMultipleInstances
        {
            get
            {
                if (_allowMultipleInstances != null) return _allowMultipleInstances.Value;

                //If there is no setting, read from config file to populate the value
                lock (SyncRoot)
                {
                    if (_allowMultipleInstances == null)
                    {
                        _allowMultipleInstances = false;
                    }
                }

                return _allowMultipleInstances.Value;
            }

            set
            {
                _allowMultipleInstances = value;
            }
        }

        public abstract ISessionConfiguration Session { get; }

        public abstract ITargetConfiguration Target { get; }
    }
}