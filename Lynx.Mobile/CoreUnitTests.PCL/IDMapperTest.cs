using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using PCLStorage;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    public class IDMapperTest : MapperTest<ID>
    {

        [SetUp]
        public virtual async Task Setup()
        {
            //create some dummy attributes
            Attribute firstname = new Attribute()
            {
                Location = "1",
                Hash = "1",
                Content = new StringContent("Olivier"),
                Description = "firstname"
            };

            Certificate firstnameCert = new Certificate()
            {
                Hash = "hash",
                Location = "location",
                OwningAttribute = firstname,
                Content = new StringContent("content"),
            };

            firstname.AddCertificate(firstnameCert);

            Attribute lastname = new Attribute()
            {
                Location = "2",
                Hash = "2",
                Content = new StringContent("Brochu Dufour"),
                Description = "lastname"
            };

            Certificate lastnameCert = new Certificate()
            {
                Hash = "hash2",
                Location = "location2",
                OwningAttribute = lastname,
                Content = new StringContent("content")
            };

            lastname.AddCertificate(lastnameCert);

            Attribute age = new Attribute()
            {
                Location = "3",
                Hash = "3",
                Content = new IntContent(24),
                Description = "age"
            };

            t = new ID();
            t.AddAttribute(firstname);
            t.AddAttribute(lastname);
            t.AddAttribute(age);

            IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync("mydata.db", CreationCollisionOption.ReplaceExisting);

            var path = file.Path;

            Mapper = new IDMapper(path, new AttributeMapper(path, new ExternalElementMapper<Certificate>(path)));
            Mapper2 = new IDMapper(path, new AttributeMapper(path, new ExternalElementMapper<Certificate>(path)));
        }

        [Test]
        public override async Task TestSaveAsync()
        {
            await base.TestSaveAsync();

            foreach (KeyValuePair<string, Attribute> kV in t.Attributes)
            {
                Assert.AreNotEqual(0, kV.Value.UID);

                foreach (KeyValuePair<string, Certificate> kV2 in kV.Value.Certificates)
                {
                    Assert.AreNotEqual(0, kV2.Value.UID);
                }
            }
        }

        [Test]
        public override async Task TestGetAsync()
        {
            await base.TestGetAsync();

            foreach (KeyValuePair<string, Attribute> kV in t.Attributes)
            {
                Assert.True(tRecovered.Attributes.ContainsKey(kV.Key));

                Attribute newAttribute = tRecovered.GetAttribute(kV.Key);
                Attribute originalAttribute = kV.Value;

                Assert.AreEqual(originalAttribute.UID, newAttribute.UID);
                Assert.AreEqual(originalAttribute.Location, newAttribute.Location);
                Assert.AreEqual(originalAttribute.Hash, newAttribute.Hash);
                Assert.AreEqual(originalAttribute.Description, newAttribute.Description);
                Assert.AreEqual(originalAttribute.Content.ToString(), newAttribute.Content.ToString());

                foreach (KeyValuePair<string, Certificate> kV2 in originalAttribute.Certificates)
                {
                    Assert.True(newAttribute.Certificates.ContainsKey(kV2.Key));

                    Certificate newCertificate = newAttribute.Certificates[kV2.Key];
                    Certificate originalCertificate = kV2.Value;

                    Assert.AreEqual(originalCertificate.UID, newCertificate.UID);
                    Assert.AreEqual(originalCertificate.Hash, newCertificate.Hash);
                    Assert.AreEqual(originalCertificate.Location, newCertificate.Location);
                    Assert.AreEqual(originalCertificate.OwningAttribute.UID, newCertificate.OwningAttribute.UID);
                    Assert.AreEqual(originalCertificate.Content.ToString(), newCertificate.Content.ToString());
                }
            }
        }
    }
}
