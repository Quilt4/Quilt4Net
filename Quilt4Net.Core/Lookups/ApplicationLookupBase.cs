﻿using System;
using System.Reflection;
using Quilt4Net.Core.DataTransfer;
using Quilt4Net.Core.Interfaces;

namespace Quilt4Net.Core.Lookups
{
    internal abstract class ApplicationLookupBase : IApplicationLookup
    {
        private readonly IHashHandler _hashHandler;
        protected readonly IConfigurationHandler ConfigurationHandler;
        protected Assembly FirstAssembly;

        protected ApplicationLookupBase(IConfigurationHandler configurationHandler, IHashHandler hashHandler)
        {
            _hashHandler = hashHandler;
            ConfigurationHandler = configurationHandler;
        }

        protected virtual Assembly GetFirstAssembly()
        {
            if (FirstAssembly == null) throw new InvalidOperationException("No first assembly has been set.");
            return FirstAssembly;
        }

        public virtual void SetFirstAssembly(Assembly firstAssembly)
        {
            if (FirstAssembly != null && !ReferenceEquals(FirstAssembly, firstAssembly))
            {
                throw new InvalidOperationException("Cannot change first assembly once it has been set.");
            }

            FirstAssembly = firstAssembly;
        }

        protected abstract DateTime? GetBuildTime();

        private string GetProjectApiKey()
        {
            return ConfigurationHandler.ProjectApiKey;
        }

        public ApplicationData GetApplicationData()
        {
            var applicationName = GetApplicationName();
            var applicationVersion = GetApplicationVersion();
            var supportToolkitNameVersion = GetSupportToolkitNameVersion();
            var buildTime = GetBuildTime();
            var projectApiKey = GetProjectApiKey();
            
            var fingerPrint = $"AI1:{_hashHandler.ToMd5Hash($"{applicationName}{applicationVersion}{supportToolkitNameVersion}{projectApiKey}{buildTime}")}";

            var application = new ApplicationData
            {
                Fingerprint = fingerPrint,
                Name = applicationName,
                BuildTime = buildTime,
                SupportToolkitNameVersion = supportToolkitNameVersion,
                Version = applicationVersion,
            };

            return application;
        }
        
        protected virtual string GetSupportToolkitNameVersion()
        {
            var currentAssembly = typeof(ApplicationLookupBase).GetTypeInfo().Assembly;
            var toolkitName = currentAssembly.GetName();
            return $"{toolkitName.Name} {toolkitName.Version}";
        }

        private string GetApplicationName()
        {
            if (!string.IsNullOrEmpty(ConfigurationHandler.ApplicationName))
            {
                return ConfigurationHandler.ApplicationName;
            }
            return GetFirstAssembly().GetName().Name;
        }

        protected virtual bool IsClickOnce => false;

        protected virtual string GetApplicationVersion()
        {
            var assemblyVersion = GetFirstAssembly().GetName().Version;
            var applicationVersion = assemblyVersion;
            return applicationVersion.ToString();
        }
    }
}