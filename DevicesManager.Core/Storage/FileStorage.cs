using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevicesManager.Core.Models.DAO;

namespace DevicesManager.Core.Storage
{
    public class FileStorage<T> : MemoryStorage<T>, IStorage<T> where T : BaseEntity
    {
        private readonly string _filename;

        public FileStorage(string filename) : base()
        {
            _filename = filename;

            try
            {
                if (!string.IsNullOrEmpty(Path.GetDirectoryName(_filename)) && !Directory.Exists(Path.GetDirectoryName(_filename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(_filename));
            }
            catch(Exception e)
            {
                throw new Exception($"Cannot create path: {Path.GetDirectoryName(_filename)}", e.InnerException);
            }

            if (File.Exists(_filename))
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<T>>(File.ReadAllText(_filename));
                _data = new ConcurrentDictionary<string, T>(data.ToDictionary(x => x.Id, x => x));
            }
        }

        public override string Add(T entity)
        {
            var result = base.Add(entity);

            SaveData();

            return result;
        }

        public override bool Update(T entity)
        {
            var result = base.Update(entity);

            if (result)
                SaveData();

            return result;
        }

        public override bool Delete(string id)
        {
            var result = base.Delete(id);

            if (result)
                SaveData();

            return result;
        }

        private void SaveData()
        {
            File.WriteAllText(_filename, System.Text.Json.JsonSerializer.Serialize(_data.Select(x => x.Value)));
        }
    }
}

