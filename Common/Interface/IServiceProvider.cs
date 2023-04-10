using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
{
    public interface IServiceProvider
    {
        Task RunAsync(string address, int port, Dictionary<string, List<string>> serviceInfos, CancellationToken cancellationToken);
        ServiceStatus Status { get; }
        string GetAddress();
        int GetPort();
        Dictionary<string, List<string>> GetServices();
    }

    public enum ServiceStatus
    {
        Stopped,
        Running,
        Stopping,
        Error
    }
}
