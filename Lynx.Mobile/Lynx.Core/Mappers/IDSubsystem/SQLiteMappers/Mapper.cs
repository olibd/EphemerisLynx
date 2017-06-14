using System;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using SQLite;

namespace Lynx.Core.Mappers.IDSubsystem.SQLiteMappers
{
    public class Mapper<T> : IMapper<T> where T : IDBSerializable, new()
    {
        private readonly string _dBFilePath;
        protected readonly SQLiteConnection _db;
        private readonly IdentityMap<int, T> _idMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lynx.Core.Mappers.IDSubsystem.Mapper`1"/> class.
        /// </summary>
        /// <param name="DBFilePath">DB File path.</param>
        public Mapper(string DBFilePath)
        {
            _db = new SQLiteConnection(DBFilePath);
            //Create table if not exist
            _db.CreateTable<T>();
            _idMap = new IdentityMap<int, T>();
        }

        /// <summary>
        /// Get object T given the UID.
        /// </summary>
        /// <returns>The object T.</returns>
        /// <param name="UID">UID.</param>
        public virtual T Get(int UID)
        {
            T obj;

            if ((obj = _idMap.Find(UID)) != null)
                return obj;
            else
            {
                obj = _db.Get<T>(UID);
                _idMap.Add(obj.UID, obj);
                return obj;
            }
        }

        /// <summary>
        /// Save the specified obj.
        /// </summary>
        /// <returns>The object MUID.</returns>
        /// <param name="obj">Object.</param>
        public virtual int Save(T obj)
        {
            return SaveToDB(obj);
        }

        /// <summary>
        /// Saves to the database.
        /// </summary>
        /// <returns>The obj MUID.</returns>
        /// <param name="obj">Object.</param>
        private int SaveToDB(T obj)
        {
            if (_db.Find<T>(obj.UID) != null)
                return _db.Update(obj);
            else
                return _db.Insert(obj);
        }
    }
}
