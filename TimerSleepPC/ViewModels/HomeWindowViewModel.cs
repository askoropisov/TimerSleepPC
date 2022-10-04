using Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimerSleepPC.ViewModels;

namespace TimerSleepPC.ViewModels
{
    public class HomeWindowViewModel : ViewModelBase
    {
        private Stopwatch _stopwatch;
        Timer timer;

        public HomeWindowViewModel()
        {
            timer = new Timer(CallBack, null, Timeout.Infinite, 5 * 1000);
        }

        private string _minute;
        public string Minuts
        {
            get => _minute;
            set
            {
                this.RaiseAndSetIfChanged(ref _minute, value);
            }
        }

        private string _hours;
        public string Hours
        {
            get => _hours;
            set
            {
                this.RaiseAndSetIfChanged(ref _hours, value);
            }
        }

        private string _time;
        public string Time
        {
            get => _time;
            set
            {
                this.RaiseAndSetIfChanged(ref _time, value);
            }
        }

        private bool _PCOff;
        public bool PCOff
        {
            get => _PCOff;
            set => this.RaiseAndSetIfChanged(ref _PCOff, value);
        }

        private TimeSpan? _remainTime;
        public TimeSpan? RemainTime
        {
            get => _remainTime;
            set => this.RaiseAndSetIfChanged(ref _remainTime, value);
        }

        public void SetTime()
        {
            [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

            SetSuspendState(false, true, true);
        }

        public void GoTimer()
        {
            PCOff = true;
            int time = Convert.ToInt32(Minuts) * 60 + Convert.ToInt32(Hours) * 3600;
            AllTime = time;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            //Timer timer2 = new Timer(CallBack, 0, 0, 1000);
            timer.Change(0, 1000);
        }

        public int AllTime { get; set; }

        public void CallBack(object? state)
        {
            RemainTime = TimeSpan.FromSeconds(AllTime) - _stopwatch.Elapsed;
            if (RemainTime.Value.TotalSeconds >= 0 && RemainTime.Value.TotalSeconds <= 1) SetTime();
            //Task.Delay(200);
        }

        public void CancelTimer()
        {
            timer.Dispose();
            _stopwatch.Stop();
            PCOff = false;
            Hours = "0";
            Minuts = "0";
        }
    }
}
