using System;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagementConsole.Application;

namespace LibraryManagementConsole
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            // Run the Console Application
            var app = serviceProvider.GetService<ConsoleApp>();
            await app.RunAsync();
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // Add HttpClient
            services.AddHttpClient();

            // Register ConsoleApp
            services.AddTransient<ConsoleApp>();

            return services;
        }
    }
}