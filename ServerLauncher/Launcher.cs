using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using ServerLauncher.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Common;
using AccountSpace;
using Authentication;
using Chat;
using Inventory;
using Player;
using Session;
using GameWorld;
using Monitoring;
using DB;
using Redis;

using ServerLauncher.gRPC;
using Common.Interface;

namespace ServerLauncher
{
    public class Launcher
    {
        private GrpcServer _gServer;
        private readonly List<Common.Interface.IServiceProvider> _servers = new List<Common.Interface.IServiceProvider>();
        private readonly List<Task> _serviceTasks = new List<Task>();
        private CancellationTokenSource _serviceCancellationToken;
        private Task _monitoringTask;
        private Dictionary<string, Config.ServiceSettings> _ServiceSettings;

        public Launcher()
        {
            Initialize();
        }

        private void Initialize()
        {
            SetConfiguration();
            StartService();
        }

        private void SetConfiguration()
        {
            // appsettings.json 파일 읽기
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("servicesettings.json", optional: false, reloadOnChange: true);

            var configuration = configurationBuilder.Build();
            var appSettings = new Config.AppSettings();
            configuration.Bind(appSettings);

            string computerName = Environment.MachineName;

            // 해당 컴퓨터에서 실행할 서비스 가져오기
            if (!appSettings.Services.TryGetValue(computerName, out _ServiceSettings))
            {
                Console.WriteLine($"No services are configured for this computer ({computerName}).");
                return;
            }
        }

        private void StartService()
        {
            _serviceCancellationToken = new CancellationTokenSource();

            // 서비스 인스턴스 생성 및 실행
            List<Task> serviceTasks = new List<Task>();

            foreach (var serviceName in _ServiceSettings)
            {
                Common.Interface.IServiceProvider service = GetServiceByName(serviceName.Key, serviceName.Value.Port, 10000);
                if (service != null)
                {
                    Console.WriteLine($"Starting {serviceName}...");
                    serviceTasks.Add(service.RunAsync(serviceName.Value.IPAddress, serviceName.Value.Port, _serviceCancellationToken.Token));
                }
                else
                {
                    Console.WriteLine($"Unknown service: {serviceName}");
                }
            }

            _monitoringTask = MonitorService();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                });
        }

        private Common.Interface.IServiceProvider GetServiceByName(string serviceName, int port, int maxConnections)
        {
            // 서비스 인스턴스 생성을 위한 예제 코드
            switch (serviceName)
            {
                case "AccountService":
                    return new AccountProvider();
                case "AuthenticationService":
                    return new AuthenticationProvider();
                case "ChatService":
                    return new ChatProvider();
                case "InventoryService":
                    return new InventoryProvider();
                case "PlayerService":
                    return new PlayerProvider();
                case "SessionService":
                    return new SessionProvider(maxConnections);
                case "GameWorldService":
                    return new WorldProvider();
                case "DBService":
                    return new DBProvider();
                case "RedisService":
                    return new RedisProvider();
                case "MonitoringService":
                    return new MonitoringProvider();
                default:
                    return null;
            }
        }

        private async Task MonitorService()
        {
            while (!_serviceCancellationToken.Token.IsCancellationRequested)
            {
                foreach (var server in _servers)
                {
                    if (server.Status == ServiceStatus.Error)
                    {
                        await RestartServiceAsync(server, _serviceCancellationToken);
                    }
                }

                await Task.Delay(5000, _serviceCancellationToken.Token);
            }
        }

        public async Task RestartServiceAsync(Common.Interface.IServiceProvider service, CancellationTokenSource serviceCancellationToken)
        {
            if (service.Status == ServiceStatus.Running || service.Status == ServiceStatus.Stopping)
            {
                serviceCancellationToken.Cancel();
                serviceCancellationToken.Dispose();

                // 기존 서비스 종료 대기
                while (service.Status != ServiceStatus.Stopped)
                {
                    await Task.Delay(100);
                }
            }

            // 새로운 CancellationTokenSource 생성
            serviceCancellationToken = new CancellationTokenSource();

            // 서비스 재시작
            await service.RunAsync(service.GetAddress(), service.GetPort(), serviceCancellationToken.Token);
        }

        private async Task StopAllService()
        {
            _serviceCancellationToken.Cancel();

            await Task.WhenAll(_serviceTasks.Concat(new[] { _monitoringTask }));
        }

        private async Task<bool> StartGrpcServer(string address, int port)
        {
            try
            {
                _gServer = new GrpcServer(address, port);
                if (_gServer == null)
                {
                    LoggingService.LogInfo("gRPC server open fail");
                    return false;
                }

                _gServer.Start();

                return true;
            }
            catch (Exception ex)
            {
                await LoggingService.LogError($"{ex} An error occurred while starting the gRPC server.");
                return false;
            }
            finally
            {
                LoggingService.LogInfo("Shutting down the server...");
            }
        }
    }
}
