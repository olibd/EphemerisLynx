using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CoreUnitTests
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
            _certFacade = new CertificateFacade(_addressFrom, "", _web3);
            _attributeFacade = new AttributeFacade(_addressFrom, "", _web3, _certFacade);
        }

        [Test]
        public async Task TestDeploy()
        {
            Attribute deployed = await DeployAttributeAsync();
            Assert.NotNull(deployed.Address);
        }

        private async Task<Attribute> DeployAttributeAsync()
        {
            Attribute dummyAttribute = new Attribute()
            {
                Hash = "I am an attribute hash",
                Location = "I am an attribute location",
            };

            Attribute deployed = await _attributeFacade.DeployAsync(dummyAttribute);
            return deployed;
        }

        [Test]
        public async Task TestGetAttribute()
        {
            string address = (await DeployAttributeAsync()).Address;
            Attribute attribute = await _attributeFacade.GetAttributeAsync(address);

            Assert.AreEqual(attribute.Hash, "I am an attribute hash");
            Assert.AreEqual(attribute.Location, "I am an attribute location");
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
            Assert.AreEqual(certs.Count, 1);
            Assert.AreEqual(newCert.Address, certs.ToArray()[0].Value.Address);
            Assert.AreEqual(newCert.Location, certs.ToArray()[0].Value.Location);

        }
    }
}
