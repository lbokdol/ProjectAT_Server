using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
{
    public interface IServiceProvider
    {
        Task RunAsync(CancellationToken cancellationToken);
        ServiceStatus Status { get; }
    }

    public enum ServiceStatus
    {
        Stopped,
        Running,
        Stopping,
        Error
    }
}
