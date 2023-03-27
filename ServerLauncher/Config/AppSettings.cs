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
    }
}
