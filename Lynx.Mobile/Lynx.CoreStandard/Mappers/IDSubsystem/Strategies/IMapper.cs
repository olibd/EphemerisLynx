﻿using System;
using System.Threading.Tasks;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Mappers.IDSubsystem.Strategies
{
    public interface IMapper<T>
    {
        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <returns>The save object MUID</returns>
        /// <param name="obj">obj</param>
        Task<int> SaveAsync(T obj);

        /// <summary>
        /// Get the object by its MUID.
        /// </summary>
        /// <returns>object</returns>
        /// <param name="MUID">MUID</param>
        Task<T> GetAsync(int MUID);
    }
}
