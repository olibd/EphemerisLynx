using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Mappers.IDSubsystem;

namespace CoreUnitTests
{
    [TestFixture()]
    public class IDMapperTest
    {
        private string _localPath;
        private ID _id;
        private Mapper<ID> _mapper;

        [SetUp]
        public void Setup()
        {
            // Get directory of test DLL
            _localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //create some dummy attributes
            Lynx.Core.Models.IDSubsystem.Attribute firstname = new Lynx.Core.Models.IDSubsystem.Attribute()
            {
                Content = new StringContent("Olivier")
            };

            Lynx.Core.Models.IDSubsystem.Attribute lastname = new Lynx.Core.Models.IDSubsystem.Attribute()
            {
                Content = new StringContent("Brochu Dufour")
            };

            Lynx.Core.Models.IDSubsystem.Attribute age = new Lynx.Core.Models.IDSubsystem.Attribute()
            {
                Content = new IntContent(24)
            };

            _id = new ID();
            _id.AddAttribute("Firstname", firstname);
            _id.AddAttribute("Lastname", lastname);
            _id.AddAttribute("Age", age);

            _mapper = new Mapper<ID>(":memory:");
        }

        [Test()]
        public void TestSave()
        {
            _mapper.Save(_id);
        }
    }
}
