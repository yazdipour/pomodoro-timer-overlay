using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace PomoTimer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Hide From Alt+Tab

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        void FormLoaded(object sender, RoutedEventArgs args)
        {
            //Variable to hold the handle for the form
            var helper = new WindowInteropHelper(this).Handle;
            //Performing some magic to hide the form from Alt+Tab
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }
        #endregion

        private enum State
        {
            Play, Pause, Relax, Init, Finished, Continue
        }

        public Settings Settings { get; set; } = new Settings();
        private TimeSpan _time = TimeSpan.FromMinutes(25);
        public MainWindow() => InitializeComponent();
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion INotifyPropertyChanged

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
            if (_counter == null || CountingIsDone) //start
            {
                UpdateUiState(State.Play);
                CreateCounter(Settings.PomoTimeMinutes).Start();
            }
            else if (!_counter.IsEnabled) //continue
            {
                _counter.IsEnabled = true;
                _counter.Start();
                UpdateUiState(State.Continue);
            }
            else //pause
            {
                UpdateUiState(State.Pause);
                _counter.Stop();
                _counter.IsEnabled = false;
            }
        }

        private void Reset_OnClick(object sender, RoutedEventArgs e)
        {
            if (_counter == null)
                return;
            _counter.Stop();
            _counter = null;
            UpdateUiState(State.Init);
        }

        private void Relax_OnClick(object sender, RoutedEventArgs e)
        {
            Reset_OnClick(null, null);
            CreateCounter(Settings.RelaxTimeMinutes).Start();
            UpdateUiState(State.Relax);
        }

        private DispatcherTimer CreateCounter(int min)
        {
            _time = TimeSpan.FromMinutes(min);
            _counter = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                _time = _time.Subtract(TimeSpan.FromSeconds(1));
                TimerLabel = _time.ToString(@"mm\:ss");
                if (!CountingIsDone)
                    return;
                UpdateUiState(State.Finished);
                _counter?.Stop();
                SoundHandler.PlaySound(@"C:\Windows\Media\Windows Proximity Notification.wav");
            }, Application.Current.Dispatcher);
            return _counter;
        }

        private bool CountingIsDone => _time.Seconds == 0 && _time.Minutes == 0;
        #endregion

        private void UpdateUiState(State st)
        {
            switch (st)
            {
                case State.Play:
                    PausePlayIcon = "";
                    Settings.BgColor = App.RED;
                    break;
                case State.Pause:
                    PausePlayIcon = "";
                    break;
                case State.Relax:
                    Settings.BgColor = App.GREEN;
                    PausePlayIcon = "";
                    break;
                case State.Init:
                    PausePlayIcon = "";
                    Settings.BgColor = App.ASPHALT;
                    TimerLabel = Settings.PomoTimeMinutes + ":00";
                    break;
                case State.Finished:
                    PausePlayIcon = "";
                    Settings.BgColor = App.YELLOW;
                    break;
                case State.Continue:
                    PausePlayIcon = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(st), st, null);
            }
            OnPropertyChanged(nameof(Settings));
        }
        private void Exit_OnClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void GoGithub_OnClick(object sender, RoutedEventArgs e)
            => System.Diagnostics.Process.Start("https://github.com/yazdipour/pomodoro-timer-overlay");

        private void Update_OnClick(object sender, RoutedEventArgs e)
            => System.Diagnostics.Process.Start("https://github.com/yazdipour/pomodoro-timer-overlay/releases");

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Window ui)
                GetWindow(ui)?.DragMove();
        }
        private void OpenSetting_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}
