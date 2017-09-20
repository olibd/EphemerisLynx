using System;
using SQLite;

namespace Lynx.Core.Models
{
    public interface IDBSerializable
    {
        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        [PrimaryKey, AutoIncrement]
        int UID { get; set; }
    }
}
