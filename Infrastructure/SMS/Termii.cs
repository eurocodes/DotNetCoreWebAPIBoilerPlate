using Core.Application.DTOs.Configurations;
using Core.Application.DTOs.Local;
using Core.Application.Interfaces.SMS;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SMS {
    [RegisterAsScoped]
    public class Termii : ISMSClient {
        private readonly Core.Application.DTOs.Configurations.Termii config;
        private readonly IHTTPRequest requestor;
        public Termii(IOptionsMonitor<SystemVariables> config, IHTTPRequest requestor) {
            this.config = config.CurrentValue.Termii;
            this.requestor = requestor;
        }
        public async Task<bool> sendSMS(string message, string phone) {
            JObject requestBody = new JObject();
            requestBody.Add("api_key", config.api_key);
            requestBody.Add("sms", message);
            requestBody.Add("to", phone);
            requestBody.Add("from", config.senderID);
            requestBody.Add("channel", "dnd");
            requestBody.Add("type", "plain");
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string contentType = "application/json";
            string url = string.Concat(config.root, config.sendSMS);
            HTTPResponse r = await requestor.client(url, requestBody.ToString(), HTTPVerb.POST, contentType, headers);
            if (r?.statusCode == 200)
                return true;
            return false;
        }

        public async Task<string> sendOTP(string phone) {
            JObject requestBody = new JObject();
            requestBody.Add("api_key", config.api_key);
            requestBody.Add("message_type", "NUMERIC");
            requestBody.Add("to", phone);
            requestBody.Add("from", "N-Alert");
            requestBody.Add("channel", "dnd");
            requestBody.Add("pin_attempts", 5);
            requestBody.Add("pin_time_to_live", 10);
            requestBody.Add("pin_length", 4);
            requestBody.Add("pin_placeholder", "<1234>");
            requestBody.Add("message_text", "Your TAP Confirmation code is <1234>");
            requestBody.Add("pin_type", "NUMERIC");
            string url = string.Concat(config.root, config.sendOTP);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string contentType = "application/json";
            HTTPResponse r = await requestor.client(url, requestBody.ToString(), HTTPVerb.POST, contentType, headers);
            if (r?.statusCode == 200) {
                var response = (JObject.Parse(r.response)).ToObject<TermiiOTPResponse>();
                return response.pinId;
            }
            return null;
        }

        public async Task<bool> verifyOTP(string phone, string otp, string token) {
            JObject requestBody = new JObject();
            requestBody.Add("api_key", config.api_key);
            requestBody.Add("pin_id", token);
            requestBody.Add("pin", otp);
            string url = string.Concat(config.root, config.verify);
            string contentType = "application/json";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            HTTPResponse r = await requestor.client(url, requestBody.ToString(), HTTPVerb.POST, contentType, headers);
            if (r?.statusCode == 200)
                return true;
            return false;
        }
    }

    public class TermiiOTPResponse {
        public string pinId { get; set; }
        public string to { get; set; }
        public string verifysmsStatus { get; set; }
    }
}