using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using PCLStorage;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    public class AttributeMapperTest : MapperTest<Attribute>
    {
        [SetUp]
        public virtual async Task Setup()
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
                Revoked = false,
                Location = "certLocation",
                Hash = "certHash",
                Content = new StringContent("I certify that this is his real name.")
            };

            Certificate nameCert2 = new Certificate()
            {
                Revoked = false,
                Location = "certLocation2",
                Hash = "certHash2",
                Content = new StringContent("I certify that this is his real name 2.")
            };

            t.AddCertificate(nameCert);
            t.AddCertificate(nameCert2);

            IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync("mydata.db", CreationCollisionOption.ReplaceExisting);

            var path = file.Path;

            Mapper = new AttributeMapper(path, new ExternalElementMapper<Certificate>(path));
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
