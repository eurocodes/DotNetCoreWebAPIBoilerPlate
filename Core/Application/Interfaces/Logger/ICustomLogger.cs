using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces {
    public interface ICustomLogger {
        Task<bool> log(int severity, string eventID, string message);
    }
}
