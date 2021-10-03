using System;
namespace DevicesManager.Console.Models
{
    public enum EBackendType
    {
        API,
        Queue
    }

    public class BackendSettings
    {
        public EBackendType Type { get; set; }
        public string HostAPI { get; set; }
        public QueueSettings Queue { get; set; }
    }

    public class QueueSettings
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
    }
}