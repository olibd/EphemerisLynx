using NUnit.Framework;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using System.Threading.Tasks;
using Lynx.Core.Models;

namespace CoreUnitTests
{
    [TestFixture()]
    public abstract class MapperTest<T> where T : IDBSerializable, new()
    {
        public T t { get; set; }
        public IMapper<T> Mapper { get; set; }

        [Test]
        public virtual async Task TestSaveAsync()
        {
            Assert.AreEqual(0, t.UID);
            int primaryKey = await Mapper.SaveAsync(t);
            Assert.AreEqual(primaryKey, t.UID);
        }

        [Ignore("Feature unimplemented in model classes")]
        [Test]
        public virtual async Task TestGetAsync()
        {
            await TestSaveAsync();
            Assert.AreNotEqual(0, t.UID);
            T tRecovered = await Mapper.GetAsync(t.UID);
            Assert.AreEqual(t.UID, tRecovered.UID);
        }
    }
}
