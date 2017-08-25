using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lynx.Core.Communications;
using Lynx.Core.Communications.Interfaces;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace CoreUnitTests
{
    [TestFixture]
    public class SessionTest
    {
        private ISession _session1;
        private ISession _session2;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSendAndReceiveMessage()
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            string messageReceived = "";
            string message = "This is a test message";

            _session1 = new PubNubSession(delegate { });
            _session2 = new PubNubSession(
                delegate (object sender, string eventArgs)
                {
                    resetEvent.Set();
                    messageReceived = eventArgs;
                }
            );

            string sessionKey = _session1.Open();
            _session2.Open(sessionKey);

            Task.Delay(2000).Wait();

            _session1.Send(message);
            resetEvent.WaitOne(50000);

            Assert.AreEqual(message, messageReceived);
        }

    }
}
