using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Mappers.IDSubsystem.SQLiteMappers;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Nethereum.Web3;

namespace CoreUnitTests.PCL
{
    [TestFixture]
    public class CertificateFacadeTest : FacadeTest
    {
        private ICertificateFacade _facade;

        [SetUp]
        public void Setup()
        {
            SetupAsync().Wait();
            _facade = new CertificateFacade(_web3, new DummyContentService(), _accountService);
        }

        [Test]
        public async Task TestDeployAsync()
        {
            Certificate deployed = await DeployCertificateAsync();
            Assert.NotNull(deployed.Address);
        }

        [Test]
        public async Task TestGetCertificateAsync()
        {
            string address = (await DeployCertificateAsync()).Address;
            Certificate cert = await _facade.GetCertificateAsync(address);

            Assert.AreEqual(cert.Hash, "I am a hash");
            Assert.AreEqual(cert.Location, "I am a location");
            Assert.False(cert.Revoked);
        }

        private async Task<Certificate> DeployCertificateAsync()
        {
            Attribute dummyAttribute = new Attribute()
            {
                Address = _accountService.GetAccountAddress()
            };

            Certificate cert = new Certificate()
            {
                Hash = "I am a hash",
                Location = "I am a location",
                OwningAttribute = dummyAttribute
            };

            Certificate deployed = await _facade.DeployAsync(cert);
            return deployed;
        }

        [TearDown]
        protected void Teardown()
        {

        }
    }
}
