namespace Common
{
    public class LoadBalancer<T>
    {
        private List<T> _servers;
        private int _currentServerIndex;

        public LoadBalancer(List<T> servers)
        {
            _servers = servers;
            _currentServerIndex = 0;
        }

        public T GetNextServer()
        {
            if (_servers.Count == 0)
            {
                throw new InvalidOperationException("No servers are available.");
            }

            var nextServer = _servers[_currentServerIndex];
            _currentServerIndex = (_currentServerIndex + 1) % _servers.Count;
            return nextServer;
        }

        public void AddServer(T server)
        {
            _servers.Add(server);
        }
    }
}
