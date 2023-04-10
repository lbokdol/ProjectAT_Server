﻿using Common;
using Common.Interface;
using System.Net;
using System.Runtime.CompilerServices;

namespace GameWorld
{
    public class WorldProvider : Common.Interface.IServiceProvider
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

        private ServiceStatus _status = ServiceStatus.Stopped;
        private string _address;
        private int _port;
        private Dictionary<string, List<string>> _serviceInfos;

        public async Task RunAsync(string address, int port, Dictionary<string, List<string>> serviceInfos, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());

            Initialize(address, port, serviceInfos);

            try
            {
                LoggingService.Logger.Information("World Service is Starting...");

                await _taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "World Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("World is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

        private void Initialize(string address, int port, Dictionary<string, List<string>> serviceInfos)
        {
            _address = address;
            _port = port;
            _serviceInfos = serviceInfos;
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
    }
}