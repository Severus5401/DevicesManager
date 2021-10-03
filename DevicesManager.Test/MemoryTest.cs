using System;
using System.Linq;
using DevicesManager.Core;
using DevicesManager.Core.Models.DAO;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Core.Repositories;
using DevicesManager.Core.Storage;
using Xunit;

namespace DevicesManger.Test
{
    public class MemoryTest
    {
        private readonly IDeviceRepository _deviceRepository;

        public MemoryTest()
        {
            var memoryStorage = new MemoryStorage<Device>();
            _deviceRepository = new DeviceRepository(memoryStorage);
        }

        [Fact]
        public void CreateDevice()
        {
            var deviceRequest = new DeviceRequest()
            {
                DeviceType = (short)EDeviceType.ElectricMeter,
                FirmwareVersion = "1.5b",
                State = "Runing"
            };

            //Serial number mandatory
            Assert.Throws<NullReferenceException>(() => _deviceRepository.Add(deviceRequest));

            deviceRequest.SerialNumber = "0123456789";
            Assert.True(_deviceRepository.Add(deviceRequest));

            //duplicate serial exception
            Assert.Throws<DuplicateDeviceException>(() => _deviceRepository.Add(deviceRequest));


            deviceRequest.SerialNumber = "0";
            deviceRequest.DeviceType = 9;
            Assert.Throws<UnsupportedDeviceException>(() => _deviceRepository.Add(deviceRequest));
        }

        [Fact]
        public void DeleteDevice()
        {

            var deviceRequest = new DeviceRequest()
            {
                DeviceType = (short)EDeviceType.ElectricMeter,
                SerialNumber = "0123456789",
                FirmwareVersion = "1.5b",
                State = "Runing"
            };

            Assert.True(_deviceRepository.Add(deviceRequest));

            var devices = _deviceRepository.List();
            Assert.Single(devices);

            Assert.True(_deviceRepository.Delete(devices.First().Id));

            devices = _deviceRepository.List();
            Assert.Empty(devices);
        }

        [Fact]
        public void UpdateDevice()
        {

            //Create device
            var deviceRequest = new DeviceRequest()
            {
                DeviceType = (short)EDeviceType.ElectricMeter,
                SerialNumber = "0123456789",
                FirmwareVersion = "1.5b",
                State = "Runing"
            };

            Assert.True(_deviceRepository.Add(deviceRequest));

            var devices = _deviceRepository.List();
            Assert.Single(devices);

            deviceRequest.Id = devices.First().Id;
            deviceRequest.FirmwareVersion = "2.5c";

            Assert.True(_deviceRepository.Update(deviceRequest));

            devices = _deviceRepository.List();
            Assert.Single(devices);

            Assert.Equal(deviceRequest.FirmwareVersion, devices.First().FirmwareVersion);
        }
    }
}

