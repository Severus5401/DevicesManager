using System;
namespace DevicesManager.Core.Models.Requests
{
    public enum EDeviceType
    {
        ElectricMeter = 1,
        WaterMeter = 2,
        Gateway = 3
    }

    public class DeviceRequest
    {
        public string Id { get; set; }
        public string SerialNumber { get; set; }
        public short DeviceType { get; set; }
        public string FirmwareVersion { get; set; }
        public string State { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }
}