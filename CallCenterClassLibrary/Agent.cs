﻿using System;
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
        private bool _busy;

        public bool Busy
        {
            get { return _busy; }
            set { _busy = value;
                NotifyPropertyChanged();
            }
        }

        public static event Action<Agent, Call> OnCallEnded;
        public event PropertyChangedEventHandler PropertyChanged;

        private System.Timers.Timer _timer;
        private Stopwatch _stopwatch;
        public string Name { get; private set; }
        private Call _call;
        public static int MinCallTimeInSec { get; set; }
        public static int MaxCallTimeInSec { get; set; }
        public Agent(string name)
        {
            Name = name;

            _timer = new System.Timers.Timer();
            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;

            _stopwatch = new Stopwatch();
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Busy = false;

            _stopwatch.Stop();
            _call.DurationInSec = (int)(_stopwatch.ElapsedMilliseconds / 1000);
            _stopwatch.Reset();
            OnCallEnded(this, _call);
        }

        public void TakeCall(Call call)
        {
            Busy = true;
            _call = call;

            _timer.Interval = RandomGenerator.GetRandom(MinCallTimeInSec * 1000, MaxCallTimeInSec * 1000);
            _timer.Start();
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

    }
}