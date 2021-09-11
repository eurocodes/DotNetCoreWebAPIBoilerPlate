using Core.Application.DTOs.Configurations;
using Infrastructure.Queue;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infratructure.Test {
    [TestFixture]
    public class RabbitClient {
        private protected QueueServer _config;
        [SetUp]
        public void Setup() {
            _config = new QueueServer { mqhost = "127.0.0.1", mqpw = "guest", mquser = "guest" };
        }

        [TestCase]
        public void testPublish() {
            RabbitQueueClient _client = new RabbitQueueClient(_config);
            _client.config("EmailSender");
            bool sent = _client.send("elias.akin@gmail.com");
            Assert.IsTrue(sent, "Message published to Queue Successfully");
            _client.Dispose();
            sent = _client.send("elias.akin@gmail.com");
            Assert.IsFalse(sent, "Message should not be published to Queue Since connection has been disposed");
        }
    }
}
