using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CallCenterClassLibrary
{
    public class Agent: IDisposable, INotifyPropertyChanged
    {
        #region Fields and properties
        public string Name { get; private set; }
        private Call _call;
        public static int MinCallTimeInSec { get; set; }
        public static int MaxCallTimeInSec { get; set; }
        private readonly System.Timers.Timer _timer;
        private readonly Stopwatch _stopwatch;
        private bool _busy;
        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region Events
        public static event Action<Agent, Call> OnCallEnded;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Methods
        public Agent(string name)
        {
            Name = name;

            //Initialize timer.
            _timer = new System.Timers.Timer
            {
                AutoReset = false
            };
            _timer.Elapsed += Timer_Elapsed;

            _stopwatch = new Stopwatch();
        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Busy = false;

            _stopwatch.Stop();
            _call.DurationInSec = (int)(_stopwatch.ElapsedMilliseconds / 1000);
            _stopwatch.Reset();

            //Raise event.
            OnCallEnded(this, _call);
        }
        public void TakeCall(Call call)
        {
            Busy = true;

            _call = call;

            //Start Timer.
            _timer.Interval = RandomGenerator.GetRandom(MinCallTimeInSec * 1000, MaxCallTimeInSec * 1000);
            _timer.Start();
            //Start stopwatch.
            _stopwatch.Start();
        }
        public void Dispose()
        {
            _timer.Dispose();
        }
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
