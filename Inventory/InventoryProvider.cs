﻿using Common;
using Common.Interface;
using System.Net;

namespace Inventory
{
    public class InventoryProvider : Common.Interface.IServiceProvider
    {
        private ServiceStatus _status = ServiceStatus.Stopped;
        private string _address;
        private int _port;

        public async Task RunAsync(string address, int port, CancellationToken cancellationToken)
        {
            LoggingService.Logger.Information("Inventory Service is Starting...");

            Initialize(address, port);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Inventory Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Inventory is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

        private void Initialize(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public string GetAddress()
        {
            return _address;
        }

        public int GetPort()
        {
            return _port;
        }
    }
}