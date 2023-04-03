using Grpc.Core;

namespace Common
{
    public class LoadBalancer
    {
        private List<ClientBase> _servers;
        private int _currentServerIndex;

        public LoadBalancer(List<ClientBase> servers)
        {
            _servers = servers;
            _currentServerIndex = 0;
        }

        public ClientBase GetNextServer()
        {
            if (_servers.Count == 0)
            {
                throw new InvalidOperationException("No servers are available.");
            }

            var nextServer = _servers[_currentServerIndex];
            _currentServerIndex = (_currentServerIndex + 1) % _servers.Count;
            return nextServer;
        }

        public void AddServer(ClientBase server)
        {
            _servers.Add(server);
        }
    }
}
