using System;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using SQLite;

namespace Lynx.Core.Mappers.IDSubsystem.SQLiteMappers
{
    public class Mapper<T> : IMapper<T> where T : IDBSerializable, new()
    {
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
            T obj;

            if ((obj = _idMap.Find(UID)) != null)
            {
                return obj;
            }

            SQLiteConnection db = await ConnectToTableAsync<T>();

            return await Task.Run(() =>
            {

                obj = db.Get<T>(UID);
                _idMap.Add(obj.UID, obj);
                db.Close();

                return obj;
            });
        }

        /// <summary>
        /// Save the specified obj.
        /// </summary>
        /// <returns>The object UID.</returns>
        /// <param name="obj">Object.</param>
        public virtual async Task<int> SaveAsync(T obj)
        {
            await ConnectToTableAsync<T>();

            return await Task.Run(async () =>
            {
                int retUID = await SaveToDBAsync(obj);
                _idMap.Add(obj.UID, obj);
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
            SQLiteConnection db = await ConnectToTableAsync<T>();

            return await Task.Run(() =>
            {
                int i;

                if (db.Find<T>(obj.UID) != null)
                    i = db.Update(obj);
                else
                    i = db.Insert(obj);

                db.Close();
                return i;
            });
        }

        /// <summary>
        /// Creates a table if it does not exist
        /// </summary>
        protected async Task<SQLiteConnection> CreateTableAsync<G>()
        {
            return await ConnectToTableAsync<G>();
        }

        /// <summary>
        /// Connects to table G.
        /// </summary>
        /// <typeparam name="G">The 1st type parameter.</typeparam>
        protected async Task<SQLiteConnection> ConnectToTableAsync<G>()
        {
            SQLiteConnection conn = null;
            await Task.Run(() =>
            {
                conn = new SQLiteConnection(_dbFilePath);
                //Create table if not exist
                conn.CreateTable<G>();
            });

            return conn;
        }
    }
}
