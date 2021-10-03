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
    public class LiteDBTest
    {
        private readonly string _databaseName = "test_litedb.db";
        private readonly IDeviceRepository _deviceRepository;

        public LiteDBTest()
        {
            if (System.IO.File.Exists(_databaseName))
                System.IO.File.Delete(_databaseName);

            var liteDBStorage = new LiteDBStorage<Device>(_databaseName);
            _deviceRepository = new DeviceRepository(liteDBStorage);
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

