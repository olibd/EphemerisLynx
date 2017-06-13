using System;
using SQLite.Net.Attributes;

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
