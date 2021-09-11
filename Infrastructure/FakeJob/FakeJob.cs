using Infrastructure.Interfaces.FakeJob;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FakeJob {
    [RegisterAsScoped]
    public class FakeJob : IFakeJob{
        public async Task<bool> InMaxTime(int minTime = 10, int maxTime = 1000) {
            int[] delays = Enumerable.Range(minTime, maxTime).Where(x => x % 2 == 0 && x < maxTime).ToArray();
            Random rnd = new Random();
            int selector = rnd.Next(0, delays.Count() - 1);
            await Task.Delay(delays[selector]);
            return true;
        }
    }
}
