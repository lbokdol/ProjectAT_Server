namespace ServerLauncher.Config
{
    public class AppSettings
    {
        public Dictionary<string, Dictionary<string, ServiceSettings>> Services { get; set; }
    }

    public class ServiceSettings
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public List<ServiceInfo> Services { get; set; }
    }

    public class ServiceInfo
    {
        public string Name { get; set; }
        public List<string> Address { get; set; }
    }
}
