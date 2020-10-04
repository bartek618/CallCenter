using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenterClassLibrary
{
    public class CallsGenerator: IDisposable
    {
        #region Fields and properties
        private System.Timers.Timer _timer;
        private int _minInterval;
        private int _maxInterval;
        #endregion
        #region Events
        public event Action<Call> OnCallGenerated;
        #endregion
        #region Methods
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
        public void GenerateCall()
        {
            OnCallGenerated(new Call(Guid.NewGuid().ToString()));
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GenerateCall();
            _timer.Interval = RandomGenerator.GetRandom(_minInterval * 1000, _maxInterval * 1000);
        }
        public void Dispose()
        {
            _timer.Dispose();
        } 
        #endregion
    }
}
