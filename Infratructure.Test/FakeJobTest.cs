using Core.Shared;
using Infrastructure.FakeJob;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infratructure.Test {
    [TestFixture]
    public class FakeJobTest {
        [TestCase]
        public void fakeJobShouldHaltExecution() {
            FakeJob job = new FakeJob();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            bool finished = job.InMaxTime(minTime: 100, maxTime: 500).Result;
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Assert.IsTrue(elapsedMs >= 100);
            Assert.IsTrue(elapsedMs <= 500);
        }
    }
}
