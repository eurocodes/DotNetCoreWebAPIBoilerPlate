using Core.Application.DTOs.Configurations;
using Microsoft.Extensions.Hosting;
using NetCore.AutoRegisterDi;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Queue.JobConsumer {
    [DoNotAutoRegister]
    public abstract class JobConsumer : BackgroundService {
        private ConnectionFactory _connectionFactory;
        protected IConnection _connection;
        private protected IModel _channel;
        private readonly string QueueName = string.Empty;
        private readonly QueueServer _config;
        public JobConsumer(string queueName, QueueServer config) {
            QueueName = queueName;
            _config = config;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            try {
                _connectionFactory = new ConnectionFactory {
                    HostName = _config.mqhost,
                    UserName = _config.mquser,
                    Password = _config.mqpw,
                    DispatchConsumersAsync = true
                };
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclarePassive(QueueName);
                _channel.BasicQos(0, 1, false);                
            } catch {
                
            }
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            // listen to the RabbitMQ messages
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (bc, ea) => {
                await consume(ea, bc);
            };
            try {
                //_channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            } catch { }
            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken) {
            try {
                await base.StopAsync(cancellationToken);
                _connection.Close();                
            } catch { }
        }

        public abstract Task consume(BasicDeliverEventArgs arg, object obj);
    }
}