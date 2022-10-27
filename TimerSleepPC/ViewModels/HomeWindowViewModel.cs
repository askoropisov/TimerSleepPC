using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using TimerSleepPC.Enums;

namespace TimerSleepPC.ViewModels
{
    public class HomeWindowViewModel : ViewModelBase
    {
        private Stopwatch? _stopwatch;
        private Timer? timer;

        public List<string> Mods { get; set; } = new List<string> { "Спящий режим", "Выключение", "Таймер" };

        private string? _selectMode;
        public string? SelectMode
        {
            get => _selectMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectMode, value);
                if (value == "Спящий режим") ModeS = Modes.SleepMode;
                else if (value == "Выключение") ModeS = Modes.PowerOffMode;
                else if (value == "Таймер") ModeS = Modes.TimerAlarmMode;
            }
        }

        private Modes _modeS;
        public Modes ModeS
        {
            get => _modeS;
            set => this.RaiseAndSetIfChanged(ref _modeS, value);
        }

        private string? _minute;
        public string? Minuts
        {
            get => _minute;
            set => this.RaiseAndSetIfChanged(ref _minute, value);
        }

        private string? _hours;
        public string? Hours
        {
            get => _hours;
            set => this.RaiseAndSetIfChanged(ref _hours, value);
        }

        public int AllTime { get; set; }

        private bool _PCOff;
        public bool PCOff
        {
            get => _PCOff;
            set => this.RaiseAndSetIfChanged(ref _PCOff, value);
        }

        private bool _mode = true;
        public bool Mode
        {
            get => _mode;
            set => this.RaiseAndSetIfChanged(ref _mode, value);
        }

        private TimeSpan? _remainTime;
        public TimeSpan? RemainTime
        {
            get => _remainTime;
            set => this.RaiseAndSetIfChanged(ref _remainTime, value);
        }

        public void SleepOrPowerOff()
        {
            [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

            CancelTimer();
            //if(Mode) SetSuspendState(false, true, true);
            //else Process.Start("shutdown", "/s /t 0");

            if (ModeS == Modes.SleepMode) SetSuspendState(false, true, true);
            else if (ModeS == Modes.PowerOffMode) Process.Start("shutdown", "/s /t 0");
            else if (ModeS == Modes.TimerAlarmMode) TimerAlarm();
        }

        public void TimerAlarm()
        {
            [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
            static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

            const int WM_COMMAND = 0x111;
            const int MIN_ALL = 419;
            const int MIN_ALL_UNDO = 416;

            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            //System.Threading.Thread.Sleep(2000);
            //SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);

        }

        public void GoTimer()
        {
            timer = new Timer(CallBack, null, Timeout.Infinite, 5 * 1000);
            PCOff = true;

            int time = Convert.ToInt32(Minuts) * 60 + Convert.ToInt32(Hours) * 3600;
            AllTime = time;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            timer.Change(0, 1000);
        }

        public void CallBack(object? state)
        {
            RemainTime = TimeSpan.FromSeconds(AllTime) - _stopwatch.Elapsed;
            if (RemainTime.Value.TotalSeconds >= 0 && RemainTime.Value.TotalSeconds <= 1) SleepOrPowerOff();
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
