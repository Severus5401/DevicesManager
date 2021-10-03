using System.Collections.Generic;
using DevicesManager.Core.Models.DAO;

namespace DevicesManager.Core.Storage
{
    public interface IStorage<T> where T : BaseEntity
    {
        /// <summary>
        /// Find an entity with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the entity if found, otherwise null</returns>
        T GetById(string id);

        /// <summary>
        /// Add the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>the id of the entity</returns>
        string Add(T entity);

        /// <summary>
        /// Update the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>the id of the entity</returns>
        bool Update(T entity);

        /// <summary>
        /// List all thes entities
        /// </summary>
        /// <returns>An IEnumerable of the entities that exist</returns>
        IEnumerable<T> List();

        /// <summary>
        /// Delete the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true if deleted, otherwise false</returns>
        bool Delete(string id);
    }
}