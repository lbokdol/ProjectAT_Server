using BackOffice.Service;
using Common;
using Common.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using BackOfficeService;

namespace BackOffice
{
    public class BackOfficeProvider : Common.Interface.IServiceProvider
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
        private ServiceStatus _status = ServiceStatus.Stopped;
        private string _address;
        private int _port;
        private Dictionary<string, List<string>> _serviceInfos;
        private IHostBuilder _builder;
        private IHost _app;
        private BackOfficeChannel _channel;

        public async Task RunAsync(string address, int port, Dictionary<string, List<string>> serviceInfos, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());

            Initialize(address, port, serviceInfos);

            APIServer(address, 5000);

            try
            {
                LoggingService.Logger.Information("BackOffice Service is Starting...");
                _status = ServiceStatus.Running;
                await _taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "BackOffice Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("BackOffice is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

        private void Initialize(string address, int port, Dictionary<string, List<string>> serviceInfos)
        {
            _address = address;
            _port = port;
            _serviceInfos = serviceInfos;
            _channel = new BackOfficeChannel();

            foreach(var service in serviceInfos.Keys)
            {
                _channel.AddChannel<DBServer.DBServerClient>(service, serviceInfos[service][0], int.Parse(serviceInfos[service][1]));
            }
        }

        public string GetAddress()
        {
            return _address;
        }

        public int GetPort()
        {
            return _port;
        }

        public Dictionary<string, List<string>> GetServices()
        {
            return _serviceInfos;
        }

        private void APIServer(string address, int port)
        {
            _builder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls($"http://{address}:{port}");
            });

            _app = _builder.Build();

            _app.RunAsync();

        }
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers();
                
                services.AddSingleton<BackOfficeChannel>();
                // 여기에 필요한 서비스들을 추가하십시오.
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackOffice API", Version = "v1" });
                });
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.UseHttpsRedirection();

                app.UseRouting();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackOffice API v1"));

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
        }
    }
}
