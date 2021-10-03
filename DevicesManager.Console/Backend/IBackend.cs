using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Core.Models.Responses;

namespace DevicesManager.Console.Backend
{
    public interface IBackend
    {

        /// <summary>
        /// Find an entity with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the entity if found, otherwise null</returns>
        Task<DeviceResponse> GetById(string id);

        /// <summary>
        /// Add the entity
        /// </summary>
        /// <param name="deviceRequest"></param>
        /// <returns>true if added, otherwise false</returns>
        Task<bool> Add(DeviceRequest deviceRequest);

        /// <summary>
        /// Update the entity
        /// </summary>
        /// <param name="deviceRequest"></param>
        /// <returns>the id of the entity</returns>
        Task<bool> Update(DeviceRequest deviceRequest);

        /// <summary>
        /// List all thes entities
        /// </summary>
        /// <returns>An IEnumerable of the entities that exist</returns>
        Task<IEnumerable<DeviceResponse>> List();

        /// <summary>
        /// Delete the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true if deleted, otherwise false</returns>
        Task<bool> Delete(string id);

        /// <summary>
        /// Get the supported devices available
        /// </summary>
        /// <returns>An IEnumerable</returns>
        Task<IEnumerable<SupportedDeviceResponse>> GetSupportedDevices();
    }
}

