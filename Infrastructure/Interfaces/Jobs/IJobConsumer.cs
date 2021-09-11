using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Jobs {
    public interface IJobConsumer {
        public Task consume(BasicDeliverEventArgs arg, object obj);
    }
}
