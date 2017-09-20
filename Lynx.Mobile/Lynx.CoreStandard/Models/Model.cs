using System;
using SQLite;

namespace Lynx.Core.Models
{
    public abstract class Model : IDBSerializable
    {
        /// <summary>
        /// Gets or sets the UID
        /// </summary>
        /// <value>The UID.</value>
        [PrimaryKey, AutoIncrement]
        public int UID { get; set; }
    }
}
