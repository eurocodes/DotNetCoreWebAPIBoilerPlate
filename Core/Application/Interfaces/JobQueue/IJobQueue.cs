using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Interfaces.JobQueue {
    public interface IJobQueue {
        void config(string queueName, bool persist = true, IDictionary<string, object> headers = null);
        bool send(string payload);
    }
}
