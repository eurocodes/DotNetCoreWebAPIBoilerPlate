using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.FakeJob {
    public interface IFakeJob {
        Task<bool> InMaxTime(int minTime = 10, int maxTime = 1000);
    }
}
