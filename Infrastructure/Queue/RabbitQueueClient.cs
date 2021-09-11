using Core.Application.DTOs.Configurations;
using Core.Application.Interfaces.JobQueue;
using Microsoft.Extensions.Options;
using NetCore.AutoRegisterDi;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Queue {
    [RegisterAsSingleton]
    public class RabbitQueueClient : IDisposable, IJobQueue {
        private readonly IConnection _connection;
        private IModel _channel;
        private IBasicProperties _config;
        private string _appID = string.Empty;
        public RabbitQueueClient(IOptionsMonitor<SystemVariables> config) {
            var factory = new ConnectionFactory() { HostName = config.CurrentValue.QueueServer.mqhost, UserName = config.CurrentValue.QueueServer.mquser, Password = config.CurrentValue.QueueServer.mqpw };
            this._connection = factory.CreateConnection();
        }
        public RabbitQueueClient(QueueServer config) {
            var factory = new ConnectionFactory() { HostName = config.mqhost, UserName = config.mquser, Password = config.mqpw };
            this._connection = factory.CreateConnection();
        }

        public void config(string queueName, bool persist = true, IDictionary<string, object> headers = null) {
            IDictionary<string, object> headersValueAsObj = new Dictionary<string, object>();
            this._appID = queueName;
            if(headers != null) {
                foreach (KeyValuePair<string, object> l in headers) {
                    headersValueAsObj.Add(l.Key, (object)l.Value);
                }
            }
            this._channel = _connection.CreateModel();
            _config = _channel.CreateBasicProperties();
            _config.Headers = headersValueAsObj;
            _config.Persistent = persist;
            _channel.QueueDeclare(queue: _appID,
                                 durable: persist,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }
        public bool send(string payload) {
            try {
                var _body = Encoding.UTF8.GetBytes(payload);
                _channel.BasicPublish(exchange: "",
                                     routingKey: _appID,
                                     basicProperties: _config,
                                     body: _body);
            } catch {
                return false;
            }
            return true;
        }

        private void close() {
            try {
                _connection.Close();
            } catch { }
        }

        public void Dispose() {
            close();
        }
    }
}
