using System;
using System.Collections.Generic;
using System.Linq;
using DevicesManager.Core.Models.DAO;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Core.Models.Responses;
using DevicesManager.Core.Storage;

namespace DevicesManager.Core.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IStorage<Device> _storage;

        public DeviceRepository(IStorage<Device> storage)
        {
            _storage = storage;
        }

        public DeviceResponse GetById(string id)
        {
            var entity = _storage.GetById(id);
            if (entity == null)
                return null;

            return new DeviceResponse()
            {
                Id = entity.Id,
                DeviceType = entity.DeviceType.ToString(),
                FirmwareVersion = entity.FirmwareVersion,
                IP = entity.IP,
                Port = entity.Port,
                SerialNumber = entity.SerialNumber,
                State = entity.State
            };
        }

        public bool Add(DeviceRequest deviceRequest)
        {
            if (string.IsNullOrEmpty(deviceRequest.SerialNumber)) throw new NullReferenceException($"The field {nameof(deviceRequest.SerialNumber)} is mandatory");
            if (!Enum.IsDefined(typeof(EDeviceType), (int)deviceRequest.DeviceType)) throw new UnsupportedDeviceException("The specified device is not supported");
            if (!CheckSerial(deviceRequest.SerialNumber)) throw new DuplicateDeviceException(deviceRequest.SerialNumber);

            _storage.Add(new Device()
            {
                DeviceType = deviceRequest.DeviceType,
                FirmwareVersion = deviceRequest.FirmwareVersion,
                SerialNumber = deviceRequest.SerialNumber,
                Port = deviceRequest.DeviceType == (short)EDeviceType.Gateway ? deviceRequest.Port : 0,
                IP = deviceRequest.DeviceType == (short)EDeviceType.Gateway ? deviceRequest.IP : null,
                State = deviceRequest.State,
            });

            return true;
        }

        public bool Update(DeviceRequest deviceRequest)
        {
            if (string.IsNullOrEmpty(deviceRequest.SerialNumber)) throw new NullReferenceException($"The field {nameof(deviceRequest.SerialNumber)} is mandatory");
            if (!Enum.IsDefined(typeof(EDeviceType), (int)deviceRequest.DeviceType)) throw new UnsupportedDeviceException("The specified device is not supported");

            return _storage.Update(new Device()
            {
                Id = deviceRequest.Id,
                DeviceType = deviceRequest.DeviceType,
                FirmwareVersion = deviceRequest.FirmwareVersion,
                Port = deviceRequest.DeviceType == (short)EDeviceType.Gateway ? deviceRequest.Port : 0,
                IP = deviceRequest.DeviceType == (short)EDeviceType.Gateway ? deviceRequest.IP : null,
                SerialNumber = deviceRequest.SerialNumber,
                State = deviceRequest.State,
            });
        }

        public bool Delete(string id)
        {
            return _storage.Delete(id);
        }

        public IEnumerable<SupportedDeviceResponse> GetSupportedDevices()
        {
            return Enum.GetValues(typeof(EDeviceType))
                       .Cast<EDeviceType>()
                       .Select(e => new SupportedDeviceResponse((int)e, e.ToString()));
        }

        public IEnumerable<DeviceResponse> List()
        {
            return _storage.List().Select(x =>
                new DeviceResponse()
                {
                    Id = x.Id,
                    DeviceType = ((EDeviceType)x.DeviceType).ToString(),
                    FirmwareVersion = x.FirmwareVersion,
                    IP = x.IP,
                    Port =x.Port,
                    SerialNumber = x.SerialNumber,
                    State = x.State
                });
        }


        /// <summary>
        /// Check if the serial is allready present
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private bool CheckSerial(string serialNumber)
        {
            var hash = new HashSet<string>(_storage.List().Select(x => x.SerialNumber));
            if (hash.Contains(serialNumber))
                return false;
            return true;
        }
    }
}