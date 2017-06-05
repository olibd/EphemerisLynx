using System;
using System.Collections.Generic;

namespace Lynx.Core.Mappers
{
    public class IdentityMap<K, V>
    {
        private Dictionary<K, V> _idMap;

        public IdentityMap()
        {
        }

        /// <summary>
        /// Find the value for specified key.
        /// </summary>
        /// <returns>The found value. default(V) if not found.</returns>
        /// <param name="key">Key.</param>
        public V Find(K key)
        {
            if (_idMap.ContainsKey(key))
                return _idMap[key];
            else
                return default(V);
        }

        /// <summary>
        /// Add the specified Value to the specified Key.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="key">Key.</param>
        /// <param name="val">Value.</param>
        public void Add(K key, V val)
        {
            _idMap.Add(key, val);
        }
    }
}
