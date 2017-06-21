using System;
namespace Lynx.Core.Mappers.IDSubsystem.Strategies
{
    public interface IIdentityMap<K, V>
    {
        /// <summary>
        /// Find the value for specified key.
        /// </summary>
        /// <returns>The found value. default(V) if not found.</returns>
        /// <param name="key">Key.</param>
        V Find(K key);

        /// <summary>
        /// Add the specified Value to the specified Key.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="key">Key.</param>
        /// <param name="val">Value.</param>
        void Add(K key, V val);
    }
}
