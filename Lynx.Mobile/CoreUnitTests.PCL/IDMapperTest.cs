using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    public class IDMapperTest : MapperTest<ID>
    {
        [SetUp]
        public virtual void Setup()
        {
            //create some dummy attributes
            Attribute firstname = new Attribute()
            {
                Location = "1",
                Hash = "1",
                Content = new StringContent("Olivier")
            };

            Attribute lastname = new Attribute()
            {
                Location = "2",
                Hash = "2",
                Content = new StringContent("Brochu Dufour")
            };

            Attribute age = new Attribute()
            {
                Location = "3",
                Hash = "3",
                Content = new IntContent(24)
            };

            t = new ID();
            t.AddAttribute("Firstname", firstname);
            t.AddAttribute("Lastname", lastname);
            t.AddAttribute("Age", age);

            Mapper = new IDMapper(":memory:", new AttributeMapper(":memory:", new ExternalElementMapper<Certificate>(":memory:")));
        }

        [Test]
        public override async Task TestSaveAsync()
        {
            await base.TestSaveAsync();

            foreach (KeyValuePair<string, Attribute> kV in t.Attributes)
            {
                Assert.AreNotEqual(0, kV.Value.UID);
            }
        }
    }
}
