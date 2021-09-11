using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Response {
    public class ResponseFormat {
        public class RawResponse {
            public object content { get; set; }
            public StatusIdentifier error { get; set; }
            public StatusIdentifier success { get; set; }

        }
        private class SuccessfulResponse : RawResponse {
            public new StatusIdentifier error { get; set; } = new StatusIdentifier { status = 0 };
            public new StatusIdentifier success { get; set; } = new StatusIdentifier { status = 1, message = "Successful" };
        }
        private class FailedResponse : RawResponse {
            public new StatusIdentifier error { get; set; } = new StatusIdentifier { status = 1, message = "An error occured" };
            public new StatusIdentifier success { get; set; } = null;
        }

        public RawResponse failed(string message) {
            var r = new FailedResponse();
            r.error.message = message;
            return r;
        }
#nullable enable
        public RawResponse success(string? message, object? data = null) {
            var r = new SuccessfulResponse();
            r.success.message = message ?? "Request Successful";
            if (data != null)
                r.content = data;
            return r;
        }

        public class StatusIdentifier {
            public int status { get; set; }
            public string message { get; set; } = string.Empty;
        }
    }
}
