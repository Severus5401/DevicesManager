using System;
using LiteDB;

namespace DevicesManager.Core.Models.DAO
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }
    }
}

