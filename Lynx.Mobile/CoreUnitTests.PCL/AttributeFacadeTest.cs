using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CoreUnitTests.PCL
{
    [TestFixture]
    class AttributeFacadeTest : FacadeTest
    {
        private ICertificateFacade _certFacade;
        private IAttributeFacade _attributeFacade;

        [SetUp]
        public void Setup()
        {
            SetupAsync().Wait();
            _certFacade = new CertificateFacade(_web3, new DummyContentService(), _accountService);
            _attributeFacade = new AttributeFacade(_web3, _certFacade, new DummyContentService(), _accountService);
        }

        [Test]
        public async Task TestDeploy()
        {
            Attribute deployed = await DeployAttributeAsync();
            Assert.NotNull(deployed.Address);
        }

        private async Task<Attribute> DeployAttributeAsync()
        {
            Attribute attr = new Attribute()
            {
                Hash = "I am an attribute hash",
                Location = "I am an attribute location",
                Description = "attrDescription"
            };

            Attribute deployed = await _attributeFacade.DeployAsync(attr, _accountService.GetAccountAddress());
            return deployed;
        }

        [Test]
        public async Task TestGetAttribute()
        {
            string address = (await DeployAttributeAsync()).Address;
            Attribute attribute = await _attributeFacade.GetAttributeAsync(address);

            Assert.AreEqual(attribute.Hash, "I am an attribute hash");
            Assert.AreEqual(attribute.Location, "I am an attribute location");
            Assert.AreEqual(attribute.Description, "attrDescription");
        }

        [Test]
        public async Task TestAddAndGetCertificatesAsync()
        {
            Attribute attr = await DeployAttributeAsync();

            Certificate cert = new Certificate()
            {
                Hash = "I am a certificate hash",
                Location = "I am a certificate location",
                OwningAttribute = attr
            };

            Certificate newCert = await _attributeFacade.AddCertificateAsync(attr, cert);

            Dictionary<string, Certificate> certs = await _attributeFacade.GetCertificatesAsync(attr);
            Assert.AreEqual(1, certs.Count);
            Assert.AreEqual(newCert.Address, certs.ToArray()[0].Value.Address);
            Assert.AreEqual(newCert.Location, certs.ToArray()[0].Value.Location);

        }
    }
}
