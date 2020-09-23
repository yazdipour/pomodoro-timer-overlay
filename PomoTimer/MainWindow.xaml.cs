using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace PomoTimer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _locationTop = 32;
        public string BgColor { get; set; } = "#ff0000";

        public int LocationTop
        {
            get => _locationTop;
            set
            {
                _locationTop = value;
                OnPropertyChanged(nameof(LocationTop));
            }
        }

        public int LocationLeft { get; set; } = 82;
        public float OpacityValue { get; set; } = 0.5f;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion INotifyPropertyChanged

        public MainWindow()
        {
            InitializeComponent();
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            LocationTop = 100;
            MessageBox.Show("Hs");
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
        private string _timerLabel = "20:00";
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
            if (_counter == null || !_counter.IsEnabled)
            {
                PausePlayIcon = "";
                CreateCounter(1).Start();
            }
            else
            {
                PausePlayIcon = "";
                _counter.Stop();
                _counter.IsEnabled = false;
            }
        }

        private DispatcherTimer CreateCounter(int min)
        {
            var time = TimeSpan.FromMinutes(min);
            _counter = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                time = time.Subtract(TimeSpan.FromSeconds(1));
                if (time.Seconds == 0)
                    _counter?.Stop();
                TimerLabel = time.ToString(@"mm\:ss");
            }, Application.Current.Dispatcher);
            return _counter;
        }
        #endregion
    }
}
