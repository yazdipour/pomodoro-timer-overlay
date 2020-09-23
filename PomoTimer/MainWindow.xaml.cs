using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Threading;

namespace PomoTimer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Settings Settings { get; set; } = new Settings();
        private TimeSpan _time = TimeSpan.FromMinutes(25);

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion INotifyPropertyChanged

        public MainWindow()
        {
            InitializeComponent();
        }


        private void OpenMenu_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var contextMenu = btn?.ContextMenu;
            if (contextMenu == null)
                return;
            contextMenu.PlacementTarget = btn;
            contextMenu.IsOpen = true;
            e.Handled = true;
        }

        #region Counter
        private DispatcherTimer _counter;
        private string _pausePlayIcon = "";
        public string PausePlayIcon
        {
            get => _pausePlayIcon;
            set
            {
                _pausePlayIcon = value;
                OnPropertyChanged(nameof(PausePlayIcon));
            }
        }
        private string _timerLabel = "25:00";
        public string TimerLabel
        {
            get => _timerLabel;
            set
            {
                _timerLabel = value;
                OnPropertyChanged(nameof(TimerLabel));
            }
        }

        private void StartStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (_counter == null || _time.Seconds == 0) //start
            {
                PausePlayIcon = "";
                Settings.BgColor = App.RED;
                OnPropertyChanged(nameof(Settings));
                CreateCounter(Settings.PomoTimeMinutes).Start();
            }
            else if (!_counter.IsEnabled) //continue
            {
                PausePlayIcon = "";
                _counter.IsEnabled = true;
                _counter.Start();
            }
            else //pause
            {
                PausePlayIcon = "";
                _counter.Stop();
                _counter.IsEnabled = false;
            }
        }

        private DispatcherTimer CreateCounter(int min)
        {
            _time = TimeSpan.FromMinutes(min);
            _counter = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                _time = _time.Subtract(TimeSpan.FromSeconds(1));
                TimerLabel = _time.ToString(@"mm\:ss");
                if (_time.Seconds == 0)
                    _counter?.Stop();
            }, Application.Current.Dispatcher);
            return _counter;
        }
        private void Reset_OnClick(object sender, RoutedEventArgs e)
        {
            if (_counter == null)
                return;
            _counter.Stop();
            _counter = null;
            PausePlayIcon = "";
            Settings.BgColor = App.ASPHALT;
            OnPropertyChanged(nameof(Settings));
            TimerLabel = Settings.PomoTimeMinutes + ":00";
        }
        private void Relax_OnClick(object sender, RoutedEventArgs e)
        {
            Reset_OnClick(null, null);
            Settings.BgColor = App.GREEN;
            PausePlayIcon = "";
            OnPropertyChanged(nameof(Settings));
            CreateCounter(Settings.RelaxTimeMinutes).Start();
        }
        #endregion

        private void Exit_OnClick(object sender, RoutedEventArgs e) => System.Windows.Application.Current.Shutdown();
        private void GoGithub_OnClick(object sender, RoutedEventArgs e)
            => System.Diagnostics.Process.Start("https://github.com/yazdipour/pomodoro-timer-overlay");

        private void Update_OnClick(object sender, RoutedEventArgs e)
            => System.Diagnostics.Process.Start("https://github.com/yazdipour/pomodoro-timer-overlay/releases");
        private void OpenSetting_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}
