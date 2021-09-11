using Core.Application.DTOs;
using Core.Application.DTOs.Local;
using Infrastructure.Interfaces;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HTTP {
    [RegisterAsScoped]
    public class HttpWebRequestClient : IHTTPRequest {
        public async Task<HTTPResponse> client(string url, string request, HTTPVerb verb, string contenttype, Dictionary<string, string> headers = null) {
            HTTPResponse responseObj = new HTTPResponse();
            try {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = contenttype;
                httpWebRequest.Method = verb.ToString();
                httpWebRequest.KeepAlive = false;
                if (headers != null) {
                    foreach (KeyValuePair<string, string> header in headers) {
                        httpWebRequest.Headers.Add(header.Key, header.Value);
                    }
                }
                if (request != null) {
                    using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync())) {
                        await streamWriter.WriteAsync(request);
                        await streamWriter.FlushAsync();
                        streamWriter.Close();
                    }
                }
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (HttpWebResponse response = (HttpWebResponse)await httpWebRequest.GetResponseAsync()) {
                    responseObj.statusCode = (int)response.StatusCode;
                    using (var streamReader = new StreamReader(response.GetResponseStream())) {
                        var result = await streamReader.ReadToEndAsync();
                        responseObj.response = result;
                    }
                }
            } catch (WebException err) {
                using (WebResponse response = err.Response) {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    responseObj.statusCode = (int)httpResponse.StatusCode;
                    using (Stream data = response.GetResponseStream()) {
                        string text = new StreamReader(data).ReadToEnd();
                        responseObj.response = text;
                    }
                }
            } catch (Exception err) {
                responseObj.statusCode = -1;
                responseObj.error = err;
            }
            return responseObj;
        }
    }
}
