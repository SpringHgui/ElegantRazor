using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;

namespace ElegantRazor
{
    public class ElegantRazor : IDisposable
    {
        static ServiceProvider? serviceProvider;
        static object locker = new object();

        /// <summary>
        /// ≥ı ºªØ≈‰÷√
        /// </summary>
        /// <param name="service"></param>
        /// <param name="applicationAssembly"></param>
        public void Initialize(Action<ServiceCollection> service, Assembly applicationAssembly)
        {
            var applicationName = applicationAssembly.GetName().Name!;
            Initialize(service, applicationName);
        }

        public void Initialize(Assembly applicationAssembly)
        {
            var applicationName = applicationAssembly.GetName().Name!;
            Initialize(null, applicationName);
        }

        public void Initialize(Action<ServiceCollection>? service, string applicationName)
        {
            lock (locker)
            {
                init(service, applicationName);
            }
        }

        /// <summary>
        /// ‰÷»æ“≥√Ê
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="razorFile"></param>
        /// <param name="Model"></param>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public Task<string> RenderViewAsync<T>(string razorFile, T Model, DynamicViewData viewData)
        {
            if (serviceProvider == null)
                throw new InvalidOperationException("Please Call Initialize Befor RenderView");

            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
                return helper.RenderViewToStringAsync(razorFile, Model, viewData);
            }
        }

        private void init(Action<ServiceCollection>? service, string applicationName)
        {
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, applicationName);
            service?.Invoke(services);

            if (serviceProvider != null)
            {
                serviceProvider.Dispose();
            }

            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureDefaultServices(IServiceCollection services, string applicationName)
        {
            services.AddSingleton<IWebHostEnvironment>(new WebHostingEnvironment
            {
                ApplicationName = applicationName,
            });

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddSingleton(new DiagnosticListener("AspNetCore"));
            services.AddLogging();
            services.AddMvc();
            services.AddTransient<RazorViewToStringRenderer>();
        }

        public void Dispose()
        {
            if (serviceProvider != null)
            {
                serviceProvider.Dispose();
            }
        }

        class WebHostingEnvironment : IWebHostEnvironment
        {
            public IFileProvider WebRootFileProvider { get; set; }

            public string WebRootPath { get; set; }

            public string ApplicationName { get; set; }

            public IFileProvider ContentRootFileProvider { get; set; }

            public string ContentRootPath { get; set; }

            public string EnvironmentName { get; set; }
        }
    }
}
