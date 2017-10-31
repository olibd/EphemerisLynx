using System;
using Newtonsoft.Json;
using SQLite;

namespace Lynx.Core.Models
{
    public abstract class Model : IDBSerializable
    {
        /// <summary>
        /// Gets or sets the UID
        /// </summary>
        /// <value>The UID.</value>
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int UID { get; set; }
    }
}
