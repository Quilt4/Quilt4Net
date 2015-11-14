﻿using System;
using System.Threading.Tasks;
using Tharga.Quilt4Net;

namespace Quilt4Net.Sample.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => MainAsync(args));

            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey();
        }

        static async void MainAsync(string[] args)
        {
            try
            {
                var port = 59779; //5000; //5004
                var client = new Client(new WebApiClient(new Uri(string.Format("http://localhost:{0}/", port)), new TimeSpan(0, 0, 0, 30)));
                await client.User.CreateAsync("xyz", "uuu", "daniel.bohlin@gmail.com");
                var loginResponse = await client.User.Login("xyz", "uuu");
                System.Console.WriteLine(loginResponse.PublicSessionKey);

                //await client.Project.CreateAsync("Some project");
                var projects = await client.Project.GetAllAsync();
                foreach (var project in projects)
                {
                    System.Console.WriteLine("Project: " + project.Name);
                }
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.Message + " @" + exception.StackTrace);
            }

            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey();
        }
    }
}