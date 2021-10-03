using System;
using Autofac;
using DevicesManager.Core.Models.DAO;
using DevicesManager.Core.Repositories;
using DevicesManager.Core.Storage;
using DevicesManagerWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevicesManager.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            try
            {
                queueSettings = Configuration.GetSection("Queue").Get<QueueSettings>();
            }
            catch (Exception e)
            {
                throw new Exception($"Invalid Setting section detected in appsettings: {e.Message}", e.InnerException);
            }
        }

        private QueueSettings queueSettings { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory

            if (queueSettings != null)
                services.AddHostedService<DeviceQueueService>();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            StorageSettings storage;
            try
            {
                storage = Configuration.GetSection("Storage").Get<StorageSettings>();
                if (storage == null) throw new NullReferenceException($"You must add the Storage section in appsettings.");
            }
            catch(Exception e)
            {
                throw new Exception($"Invalid Setting section detected in appsettings: {e.Message}", e.InnerException);
            }

            if (storage.Type == EStorageType.Memory)
            {
                builder.RegisterType<MemoryStorage<Device>>().As<IStorage<Device>>().SingleInstance().AutoActivate();
            }
            else if (storage.Type == EStorageType.File)
            {
                if (string.IsNullOrEmpty(storage.Filename)) throw new NullReferenceException($"The field <{nameof(storage.Filename)}> of Storage setting is mandatory on File Type");
                builder.RegisterType<FileStorage<Device>>().As<IStorage<Device>>().WithParameter("filename", storage.Filename).SingleInstance().AutoActivate();
            }
            else if (storage.Type == EStorageType.LiteDB)
            {
                if (string.IsNullOrEmpty(storage.Filename)) throw new NullReferenceException($"The field <{nameof(storage.Filename)}> of Storage setting is mandatory on LiteDB Type");
                builder.RegisterType<LiteDBStorage<Device>>().As<IStorage<Device>>().WithParameter("filename", storage.Filename).SingleInstance().AutoActivate().OnActivated(x =>
                {
                    x.Instance.CreateIndex(x => x.SerialNumber);
                });
            }

            builder.Register(x => { return queueSettings; }).SingleInstance();
            builder.RegisterType<DeviceRepository>().As<IDeviceRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}

