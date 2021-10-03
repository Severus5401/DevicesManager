using System;

namespace DevicesManager.Core.Models.DAO
{
    public class Device : BaseEntity
    {
        public string SerialNumber { get; set; }
        public short DeviceType { get; set; }
        public string FirmwareVersion { get; set; }
        public string State { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }
}