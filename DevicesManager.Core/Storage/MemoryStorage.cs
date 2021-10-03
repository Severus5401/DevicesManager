using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DevicesManager.Core.Models.DAO;

namespace DevicesManager.Core.Storage
{
    public class MemoryStorage<T> : IStorage<T> where T : BaseEntity
    {
        protected ConcurrentDictionary<string, T> _data = new ConcurrentDictionary<string, T>();

        public MemoryStorage()
        {
        }

        public virtual T GetById(string id)
        {
            if (!_data.ContainsKey(id))
                return null;
            return _data[id];
        }

        public virtual string Add(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            while(_data.ContainsKey(entity.Id))
                entity.Id = Guid.NewGuid().ToString();

            _data[entity.Id] = entity;

            return entity.Id;
        }

        public virtual bool Update(T entity)
        {
            if (!_data.ContainsKey(entity.Id))
                return false;

            _data[entity.Id] = entity;
            return true;
        }

        public virtual IEnumerable<T> List()
        {
            return _data.Select(x => x.Value);
        }

        public virtual bool Delete(string id)
        {
            return _data.TryRemove(id, out var output);
        }
    }
}

