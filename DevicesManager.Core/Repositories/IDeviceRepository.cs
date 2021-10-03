using System.Collections.Generic;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Core.Models.Responses;

namespace DevicesManager.Core.Repositories
{
    public interface IDeviceRepository
    {

        /// <summary>
        /// Find an entity with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the entity if found, otherwise null</returns>
        DeviceResponse GetById(string id);

        /// <summary>
        /// Add the entity
        /// </summary>
        /// <param name="deviceRequest"></param>
        /// <returns>true if added, otherwise false</returns>
        bool Add(DeviceRequest deviceRequest);

        /// <summary>
        /// Update the entity
        /// </summary>
        /// <param name="deviceRequest"></param>
        /// <returns>the id of the entity</returns>
        bool Update(DeviceRequest deviceRequest);

        /// <summary>
        /// Delete the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true if deleted, otherwise false</returns>
        bool Delete(string id);

        /// <summary>
        /// List all thes entities
        /// </summary>
        /// <returns>An IEnumerable of the entities that exist</returns>
        IEnumerable<DeviceResponse> List();

        /// <summary>
        /// Get the supported devices available
        /// </summary>
        /// <returns>An IEnumerable</returns>
        IEnumerable<SupportedDeviceResponse> GetSupportedDevices();
    }
}