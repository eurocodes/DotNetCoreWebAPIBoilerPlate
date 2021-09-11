using Core.Application.DTOs.Local;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces {
    public interface IEmailService {
        Task<bool> send(MailEnvelope envelope);
    }
}
