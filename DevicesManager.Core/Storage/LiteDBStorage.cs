using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevicesManager.Core.Models.DAO;
using LiteDB;

namespace DevicesManager.Core.Storage
{
    public class LiteDBStorage<T> : LiteDatabase, IStorage<T> where T : BaseEntity
    {
        private readonly ILiteCollection<T> _collection;

        public LiteDBStorage(string filename) :
            base(new ConnectionString(System.IO.Path.Combine(filename)) { Connection = ConnectionType.Direct })
        {
            _collection = GetCollection<T>(typeof(T).Name);
        }

        public T GetById(string id)
        {
            return _collection.FindById(new BsonValue(id));
        }

        public string Add(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            while (GetById(entity.Id) != null)
                entity.Id = Guid.NewGuid().ToString();

            _collection.Insert(new BsonValue(entity.Id), entity);

            this.Checkpoint();

            return entity.Id;
        }

        public bool Update(T entity)
        {
            return _collection.Update(new BsonValue(entity.Id), entity);
        }

        public bool Delete(string id)
        {
            return _collection.Delete(new BsonValue(id));
        }

        public virtual IEnumerable<T> List()
        {
            return _collection.FindAll();
        }

        public void CreateIndex(Expression<Func<T, object>> expression)
        {
            _collection.EnsureIndex(expression);
        }
    }
}

