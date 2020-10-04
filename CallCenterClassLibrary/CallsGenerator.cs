﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenterClassLibrary
{
    public class CallsGenerator: IDisposable
    {
        private System.Timers.Timer _timer;
        private int _minInterval;
        private int _maxInterval;

        public event Action<Call> OnCallGenerated;
        public CallsGenerator(int minCallsIntervalInSec, int maxCallsIntervalInSec)
        {
            _minInterval = minCallsIntervalInSec;
            _maxInterval = maxCallsIntervalInSec;


            _timer = new System.Timers.Timer(RandomGenerator.GetRandom(_minInterval * 1000, _maxInterval * 1000));
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
        }
        public void StartGeneratingContinously()
        {
            _timer.Start();
        }
        public void StopGeneratingContinously()
        {
            _timer.Stop();
        }
        public void Dispose()
        {
            _timer.Dispose();
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GenerateCall();
            _timer.Interval = RandomGenerator.GetRandom(_minInterval * 1000, _maxInterval * 1000);
        }
        public void GenerateCall()
        {
            OnCallGenerated(new Call(Guid.NewGuid().ToString()));
        }
    }
}
