using System;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using PCLStorage;

namespace CoreUnitTests.PCL
{
    public class ExternalElementMapperTest : MapperTest<ExternalElement>
    {
        [SetUp]
        public virtual async Task SetupAsync()
        {
            t = new ExternalElement()
            {
                Address = "0x01",
                Location = "1",
                Hash = "1",
                Owner = "0x02",
                Content = new StringContent("12345")
            };

            IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync("mydata.db", CreationCollisionOption.ReplaceExisting);

            string path = file.Path;
            Mapper = new ExternalElementMapper<ExternalElement>(path);
        }

        [Test]
        public async Task SaveAsyncTest()
        {
            Assert.AreEqual(0, t.UID);
            Assert.AreEqual(0, t.Content.UID);
            await Mapper.SaveAsync(t);
            Assert.AreEqual(1, t.UID);
            Assert.AreEqual("12345", t.Content.ToString());
        }

        [Test]
        public async Task GetAsyncTest()
        {
            await SaveAsyncTest();
            ExternalElement newElement = await Mapper.GetAsync(1);
            Assert.AreEqual(t.Address, newElement.Address);
            Assert.AreEqual(t.Location, newElement.Location);
            Assert.AreEqual(t.Hash, newElement.Hash);
            Assert.AreEqual(t.Owner, newElement.Owner);
            Assert.AreEqual(t.Content.ToString(), newElement.Content.ToString());
        }
    }
}
