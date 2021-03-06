﻿using System;
using Quilt4Net.Core.Events;
using Quilt4Net.Sample.Console.Commands.Invitation;
using Quilt4Net.Sample.Console.Commands.Issue;
using Quilt4Net.Sample.Console.Commands.Project;
using Quilt4Net.Sample.Console.Commands.Service;
using Quilt4Net.Sample.Console.Commands.Session;
using Quilt4Net.Sample.Console.Commands.Setting;
using Quilt4Net.Sample.Console.Commands.User;
using Quilt4Net.Sample.Console.Commands.Web;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Quilt4Net.Sample.Console
{
    static class Program
    {
        private static RootCommand _rootCommand;
        private static IConsole _console;

        [STAThread]
        static void Main(string[] args)
        {
            _console = new ClientConsole();

            //Note: Using the singleton version
            //var configuration = Singleton.Configuration.Instance;
            //var client = Singleton.Quilt4NetClient.Instance;
            //var sessionHandler = Singleton.Session.Instance;
            //var issueHandler = Singleton.Issue.Instance;

            //Note: Using the created instance version
            var configuration = new Configuration();
            var client = new Quilt4NetClient(configuration);
            var sessionHandler = new SessionHandler(client);
            var issueHandler = new IssueHandler(sessionHandler);

            //Note: Config in code
            //configuration.Enabled = true; //Turn the entire quilt4Net feature on or off.
            //configuration.ProjectApiKey = "9XG02ZE0BR1OI75IVX446B59M13RKBR_"; //TODO: Replace with your own ProjectApiKey.
            //configuration.ApplicationName = "MyOverrideApplication"; //Overrides the name of the assembly
            //configuration.ApplicationVersion = "MyOverrideVersion"; //Overrides the version of the assembly
            //configuration.UseBuildTime = false; //If true, separate 'versions' for each build of the assembly will be logged, even though the version number have not changed.
            //configuration.Session.Environment = "Test"; //Use dev, test, production or any other verb you like to filter on.
            //configuration.Target.Location = "http://localhost:29660"; //Address to the target service.
            //configuration.Target.Timeout = new TimeSpan(0, 0, 60);

            _console.Output(new WriteEventArgs("Connecting to quilt4 server " + configuration.Target.Location, OutputLevel.Information));

            sessionHandler.SessionRegistrationStartedEvent += Session_SessionRegistrationStartedEvent;
            sessionHandler.SessionRegistrationCompletedEvent += SessionSessionRegistrationCompletedEvent;
            sessionHandler.SessionEndStartedEvent += Session_SessionEndStartedEvent;
            sessionHandler.SessionEndCompletedEvent += Session_SessionEndCompletedEvent;
            issueHandler.IssueRegistrationStartedEvent += Issue_IssueRegistrationStartedEvent;
            issueHandler.IssueRegistrationCompletedEvent += Issue_IssueRegistrationCompletedEvent;
            client.WebApiClient.AuthorizationChangedEvent += WebApiClient_AuthorizationChangedEvent;
            client.WebApiClient.WebApiRequestEvent += WebApiClientWebApiRequestEvent;
            client.WebApiClient.WebApiResponseEvent += WebApiClient_WebApiResponseEvent;

            _rootCommand = new RootCommand(_console);
            _rootCommand.RegisterCommand(new UserCommands(client));
            _rootCommand.RegisterCommand(new ProjectCommands(client));
            _rootCommand.RegisterCommand(new InvitationCommands(client));
            _rootCommand.RegisterCommand(new SessionCommands(sessionHandler));
            _rootCommand.RegisterCommand(new IssueCommands(issueHandler));
            _rootCommand.RegisterCommand(new SettingCommands(client));
            _rootCommand.RegisterCommand(new ServiceCommands(client));
            _rootCommand.RegisterCommand(new WebCommands(issueHandler, client.WebApiClient));
            new CommandEngine(_rootCommand).Start(args);

            sessionHandler.Dispose();
        }

        private static void WebApiClient_WebApiResponseEvent(object sender, Core.Interfaces.WebApiResponseEventArgs e)
        {
            if (e.IsSuccess)
            {
                _console.Output(new WriteEventArgs($"Got response from WebAPI call after {e.Elapsed.TotalSeconds} sec.", OutputLevel.Event));
            }
            else
            {
                _console.OutputError(e.Exception);
            }
        }

        private static void WebApiClientWebApiRequestEvent(object sender, Core.Interfaces.WebApiRequestEventArgs e)
        {
            _console.Output(new WriteEventArgs($"Sending WebAPI call to {e.Path}.", OutputLevel.Event));
        }

        private static void Session_SessionRegistrationStartedEvent(object sender, SessionRegistrationStartedEventArgs e)
        {
            _console.Output(new WriteEventArgs("Starting to register session.", OutputLevel.Event));
        }

        private static void SessionSessionRegistrationCompletedEvent(object sender, SessionRegistrationCompletedEventArgs e)
        {
            if (e.Result.IsSuccess)
            {
                var message = $"Session {e.Result.ErrorMessage ?? "registered in"} {e.Result.Elapsed.TotalMilliseconds.ToString("0")}ms.";
                _console.Output(new WriteEventArgs(message, OutputLevel.Event));
            }
            else
            {
                _console.OutputError(e.Result.Exception);
            }
        }

        private static void Session_SessionEndStartedEvent(object sender, SessionEndStartedEventArgs e)
        {
            _console.Output(new WriteEventArgs("Starting to end session.", OutputLevel.Event));
        }

        private static void Session_SessionEndCompletedEvent(object sender, SessionEndCompletedEventArgs e)
        {
            if (e.Result.IsSuccess)
            {
                var message = $"Session {e.Result.ErrorMessage ?? "ended in"} {e.Result.Elapsed.TotalMilliseconds.ToString("0")}ms.";
                _console.Output(new WriteEventArgs(message, OutputLevel.Event));
            }
            else
            {
                _console.OutputError(e.Result.Exception);
            }
        }

        private static void Issue_IssueRegistrationStartedEvent(object sender, IssueRegistrationStartedEventArgs e)
        {
            _console.Output(new WriteEventArgs("Starting to register issue.", OutputLevel.Event));
        }

        private static void Issue_IssueRegistrationCompletedEvent(object sender, IssueRegistrationCompletedEventArgs e)
        {
            if (e.Result.IsSuccess)
            {
                var message = $"Issue {e.Result.ErrorMessage ?? "registered in"} {e.Result.Elapsed.TotalMilliseconds.ToString("0")}ms.";
                _console.Output(new WriteEventArgs(message, OutputLevel.Event));
            }
            else
            {
                _console.OutputError(e.Result.Exception);
            }
        }

        private static void WebApiClient_AuthorizationChangedEvent(object sender, AuthorizationChangedEventArgs e)
        {
            _console.Output(new WriteEventArgs($"Authorization changed to {(e.IsAuthorized ? "authorized" : "unauthorized")}.", OutputLevel.Event));
        }
    }
}