using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests
{
    [TestFixture()]
    public class IDMapperTest
    {
        private string _localPath;
        private ID _id;
        private IMapper<ID> _mapper;

        [SetUp]
        public void Setup()
        {
            // Get directory of test DLL
            _localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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

            firstname.AddCertificate(nameCert);
            firstname.AddCertificate(nameCert2);

            _id = new ID();
            _id.AddAttribute("Firstname", firstname);
            _id.AddAttribute("Lastname", lastname);
            _id.AddAttribute("Age", age);

            //_mapper = new IDMapper("/Users/olivier/Downloads/test.db");
            _mapper = new IDMapper(":memory:", new AttributeMapper(":memory:", new ExternalElementMapper<Certificate>(":memory:")));
        }

        [Test()]
        public void TestSave()
        {
            Assert.AreEqual(0, _id.UID);
            int primaryKey = _mapper.Save(_id);
            Assert.AreEqual(primaryKey, _id.UID);
        }
    }
}
