using Core.Application.DTOs.Configurations;
using Core.Application.DTOs.Local;
using Core.Application.Interfaces;
using Infrastructure.Interfaces.Jobs;
using Microsoft.Extensions.Options;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queue.JobConsumer {
    [DoNotAutoRegister]
    public class EmailSender : JobConsumer, IJobConsumer {
        private readonly IEmailService _emailService;
        public EmailSender(IOptionsMonitor<SystemVariables> config, IEmailService _emailService) : base(config.CurrentValue.QueueServer.Jobs.emailTrigger, config.CurrentValue.QueueServer) {
            this._emailService = _emailService;
        }

        public override async Task consume(BasicDeliverEventArgs arg, object obj) {
            var body = arg.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            try {
                var envelope = JObject.Parse(message).ToObject<MailEnvelope>();
                await _emailService.send(envelope);
                _channel.BasicAck(arg.DeliveryTag, multiple: false);
            } catch {
                _channel.BasicNack(arg.DeliveryTag, false, requeue: false);
            }
            
        }
    }
}
