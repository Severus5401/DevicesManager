using System;
namespace DevicesManager.Core
{
    public class UnsupportedDeviceException : Exception
    {
        public UnsupportedDeviceException() { }

        public UnsupportedDeviceException(string msg) : base(msg)
        {
        }
    }

    public class DuplicateDeviceException : Exception
    {
        public DuplicateDeviceException() { }

        public DuplicateDeviceException(string serialNumber) : base($"There is already a device with the serial {serialNumber}")
        {
        }
    }
}

