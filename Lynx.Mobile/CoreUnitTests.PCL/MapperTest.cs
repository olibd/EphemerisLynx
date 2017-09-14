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
        public T tRecovered { get; set; }
        public IMapper<T> Mapper { get; set; }
        public IMapper<T> Mapper2 { get; set; }

        [Test]
        public virtual async Task TestSaveAsync()
        {
            Assert.AreEqual(0, t.UID);
            int primaryKey = await Mapper.SaveAsync(t);
            Assert.AreEqual(primaryKey, t.UID);
        }


        public virtual async Task TestGetAsync()
        {
            await TestSaveAsync();
            tRecovered = await Mapper2.GetAsync(t.UID);
            Assert.AreEqual(t.UID, tRecovered.UID);
        }
    }
}
