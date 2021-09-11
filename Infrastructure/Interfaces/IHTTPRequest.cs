using Core.Application.DTOs.Local;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces {
    public interface IHTTPRequest {
        Task<HTTPResponse> client(string url, string request, HTTPVerb verb, string contenttype, Dictionary<string, string> headers = null);
    }
}
