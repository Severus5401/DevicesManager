using System;
namespace DevicesManager.Core.Models.Responses
{
    public class DeviceResponse
    {
        public string Id { get; set; }
        public string SerialNumber { get; set; }
        public string DeviceType { get; set; }
        public string FirmwareVersion { get; set; }
        public string State { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }
}

