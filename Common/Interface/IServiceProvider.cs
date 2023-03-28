using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
{
    public interface IServiceProvider
    {
        Task RunAsync(string address, int port, CancellationToken cancellationToken);
        ServiceStatus Status { get; }
        string GetAddress();
        int GetPort();
    }

    public enum ServiceStatus
    {
        Stopped,
        Running,
        Stopping,
        Error
    }
}
