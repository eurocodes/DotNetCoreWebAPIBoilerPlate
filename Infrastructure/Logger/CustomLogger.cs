using Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Logger {
    public class CustomLogger : ICustomLogger {
        public async Task<bool> log(int severity, string eventID, string message) {
            return true;
        }
    }
}
