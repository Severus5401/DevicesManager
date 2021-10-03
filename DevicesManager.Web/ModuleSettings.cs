using System;
namespace DevicesManager.Web
{
    public enum EStorageType
    {
        File,
        LiteDB,
        Memory
    }

    public class StorageSettings
    {
        public EStorageType Type { get; set; }
        public string Filename { get; set; }
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