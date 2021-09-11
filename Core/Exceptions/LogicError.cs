using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions {
    public class LogicError : Exception {
        public LogicError(string message): base(message) { }
    }
}
