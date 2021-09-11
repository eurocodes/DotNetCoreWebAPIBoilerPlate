using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.SMS {
    public interface ISMSClient {
        Task<bool> sendSMS(string message, string phone);
        Task<string> sendOTP(string phone);
        Task<bool> verifyOTP(string phone, string otp, string token);
    }
}