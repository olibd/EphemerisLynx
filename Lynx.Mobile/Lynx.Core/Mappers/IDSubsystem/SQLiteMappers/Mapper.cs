using System;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using SQLite;

namespace Lynx.Core.Mappers.IDSubsystem.SQLiteMappers
{
    public class Mapper<T> : IMapper<T> where T : IDBSerializable, new()
    {
        private readonly string _dBFilePath;
        private SQLiteConnection _db;
        protected readonly string _dbFilePath;
        private readonly IIdentityMap<int, T> _idMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lynx.Core.Mappers.IDSubsystem.Mapper`1"/> class.
        /// </summary>
        /// <param name="DBFilePath">DB File path.</param>
        public Mapper(string DBFilePath)
        {
            _dbFilePath = DBFilePath;
            _idMap = new IdentityMap<int, T>();
        }

        /// <summary>
        /// Get object T given the UID.
        /// </summary>
        /// <returns>The object T.</returns>
        /// <param name="UID">UID.</param>
        public virtual async Task<T> GetAsync(int UID)
        {
            await ConnectToTableAsync<T>();

            return await Task.Run(() =>
            {
                T obj;

                if ((obj = _idMap.Find(UID)) != null)
                {
                    _db.Close();
                    return obj;
                }
                else
                {
                    obj = _db.Get<T>(UID);
                    _idMap.Add(obj.UID, obj);
                    _db.Close();
                    return obj;
                }
            });
        }

        /// <summary>
        /// Save the specified obj.
        /// </summary>
        /// <returns>The object MUID.</returns>
        /// <param name="obj">Object.</param>
        public virtual async Task<int> SaveAsync(T obj)
        {
            await ConnectToTableAsync<T>();

            return await Task.Run(async () =>
            {
                int retUID = await SaveToDBAsync(obj);
                _db.Close();
                return retUID;
            });
        }

        /// <summary>
        /// Saves to the database.
        /// </summary>
        /// <returns>The obj UID.</returns>
        /// <param name="obj">Object.</param>
        private async Task<int> SaveToDBAsync(T obj)
        {
            return await Task.Run(() =>
            {
                if (_db.Find<T>(obj.UID) != null)
                    return _db.Update(obj);
                else
                    return _db.Insert(obj);
            });
        }

        /// <summary>
        /// Connects to table G.
        /// </summary>
        /// <typeparam name="G">The 1st type parameter.</typeparam>
        private async Task ConnectToTableAsync<G>()
        {
            await Task.Run(() =>
            {
                _db = new SQLiteConnection(_dbFilePath);
                //Create table if not exist
                _db.CreateTable<G>();
            });
        }
    }
}
