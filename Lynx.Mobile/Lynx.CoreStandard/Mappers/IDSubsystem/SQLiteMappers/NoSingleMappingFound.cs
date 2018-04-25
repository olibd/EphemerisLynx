using System;

namespace Lynx.Core.Mappers.IDSubsystem.SQLiteMappers
{
    internal class NoSingleMappingFound : Exception
    {
        public NoSingleMappingFound()
        {
        }

        public NoSingleMappingFound(string message) : base(message)
        {
        }

        public NoSingleMappingFound(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}