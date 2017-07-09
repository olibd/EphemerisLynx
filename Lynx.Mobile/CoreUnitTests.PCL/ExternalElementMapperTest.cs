using System;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;

namespace CoreUnitTests.PCL
{
    public class ExternalElementMapperTest : MapperTest<ExternalElement>
    {
        [SetUp]
        public virtual void Setup()
        {
            t = new ExternalElement()
            {
                Location = "1",
                Hash = "1",
                Content = new StringContent("Olivier")
            };

            Mapper = new ExternalElementMapper<ExternalElement>(":memory:");
        }
    }
}
