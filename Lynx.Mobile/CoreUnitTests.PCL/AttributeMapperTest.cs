using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    public class AttributeMapperTest : MapperTest<Attribute>
    {
        [SetUp]
        public virtual void Setup()
        {
            //create some dummy attributes
            t = new Attribute()
            {
                Location = "1",
                Hash = "1",
                Content = new StringContent("Olivier")
            };
            Certificate nameCert = new Certificate()
            {
                Owner = "0x00a329c0648769A73afAc7F9381E08FB43dBEA72",
                Revoked = false,
                Location = "certLocation",
                Hash = "certHash",
                Content = new StringContent("I certify that this is his real name.")
            };

            Certificate nameCert2 = new Certificate()
            {
                Owner = "0x1FD8397e8108ada12eC07976D92F773364ba46e7",
                Revoked = false,
                Location = "certLocation2",
                Hash = "certHash2",
                Content = new StringContent("I certify that this is his real name 2.")
            };

            t.AddCertificate(nameCert);
            t.AddCertificate(nameCert2);

            Mapper = new AttributeMapper(":memory:", new ExternalElementMapper<Certificate>(":memory:"));
        }

        [Test]
        public override async Task TestSaveAsync()
        {
            await base.TestSaveAsync();

            foreach (KeyValuePair<string, Certificate> kV in t.Certificates)
            {
                Assert.AreNotEqual(0, kV.Value.UID);
            }
        }
    }
}
