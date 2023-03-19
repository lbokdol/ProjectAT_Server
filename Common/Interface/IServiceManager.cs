namespace Common.Interface
{
    public interface IServiceManager
    {
        void Initialize() { }
        void Shutdown() { }
        void Connect() { }
        void Disconnect() { }
    }
}
