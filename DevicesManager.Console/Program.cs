using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DevicesManager.Console.Backend;
using DevicesManager.Console.Handler;
using DevicesManager.Console.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevicesManager.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((hostContext, builder) =>
                {
                    var configuration = hostContext.Configuration;
                    BackendSettings backend;
                    try
                    {
                        backend = configuration.GetSection("Backend").Get<BackendSettings>();
                        if (backend == null) throw new NullReferenceException($"You must add the Backend section in appsettings.");
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Invalid Setting section detected in appsettings: {e.Message}", e.InnerException);
                    }

                    if (backend.Type == EBackendType.API)
                    {
                        if (String.IsNullOrEmpty(backend.HostAPI)) throw new NullReferenceException($"You provide an HostAPI in appsettings.");
                        builder.RegisterType<APIBackend>().As<IBackend>().WithParameter("host", backend.HostAPI).SingleInstance().AutoActivate();
                    }
                    else if (backend.Type == EBackendType.Queue)
                    {
                        if (backend.Queue == null) throw new NullReferenceException($"You must add the Queue settings in appsettings.");
                        if (String.IsNullOrEmpty(backend.HostAPI)) throw new NullReferenceException($"You provide an HostAPI in appsettings.");
                        builder.RegisterType<QueueBackend>().As<IBackend>().WithParameter("settings", backend).SingleInstance().AutoActivate();
                    }

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<DeviceHandler>();
                })
                .Build();

            await host.StartAsync();
            await host.WaitForShutdownAsync();

            System.Console.WriteLine("Good Bye");
        }
    }
}

