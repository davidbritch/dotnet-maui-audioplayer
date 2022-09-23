using System.ComponentModel;

namespace AudioDemos.Controls
{
    public class Audio : View, IAudioController
    {
        #region Bindable Properties

        public static readonly BindableProperty AreTransportControlsEnabledProperty =
            BindableProperty.Create(nameof(AreTransportControlsEnabled), typeof(bool), typeof(Audio), true);

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(AudioSource), typeof(Audio), null);

        public static readonly BindableProperty AutoPlayProperty =
            BindableProperty.Create(nameof(AutoPlay), typeof(bool), typeof(Audio), true);

        private static readonly BindablePropertyKey StatusPropertyKey =
            BindableProperty.CreateReadOnly(nameof(Status), typeof(AudioStatus), typeof(Audio), AudioStatus.NotReady);

        public static readonly BindableProperty StatusProperty = StatusPropertyKey.BindableProperty;

        private static readonly BindablePropertyKey DurationPropertyKey =
            BindableProperty.CreateReadOnly(nameof(Duration), typeof(TimeSpan), typeof(Audio), new TimeSpan(),
                propertyChanged: (bindable, oldValue, newValue) => ((Audio)bindable).SetTimeToEnd());

        public static readonly BindableProperty DurationProperty = DurationPropertyKey.BindableProperty;

        public static readonly BindableProperty PositionProperty =
            BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(Audio), new TimeSpan(),
                propertyChanged: (bindable, oldValue, newValue) => ((Audio)bindable).SetTimeToEnd());

        private static readonly BindablePropertyKey TimeToEndPropertyKey =
            BindableProperty.CreateReadOnly(nameof(TimeToEnd), typeof(TimeSpan), typeof(Audio), new TimeSpan());

        public static readonly BindableProperty TimeToEndProperty = TimeToEndPropertyKey.BindableProperty;

        public bool AreTransportControlsEnabled
        {
            get { return (bool)GetValue(AreTransportControlsEnabledProperty); }
            set { SetValue(AreTransportControlsEnabledProperty, value); }
        }

        [TypeConverter(typeof(AudioSourceConverter))]
        public AudioSource Source
        {
            get { return (AudioSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }

        public AudioStatus Status
        {
            get { return (AudioStatus)GetValue(StatusProperty); }
        }

        AudioStatus IAudioController.Status
        {
            get { return Status; }
            set { SetValue(StatusPropertyKey, value); }
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
        }

        TimeSpan IAudioController.Duration
        {
            get { return Duration; }
            set { SetValue(DurationPropertyKey, value); }
        }

        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public TimeSpan TimeToEnd
        {
            get { return (TimeSpan)GetValue(TimeToEndProperty); }
            private set { SetValue(TimeToEndPropertyKey, value); }
        }

        #endregion

        #region Events

        public event EventHandler UpdateStatus;
        public event EventHandler<AudioPositionEventArgs> PlayRequested;
        public event EventHandler<AudioPositionEventArgs> PauseRequested;
        public event EventHandler<AudioPositionEventArgs> StopRequested;

        #endregion

        IDispatcherTimer _timer;

        public Audio()
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        ~Audio() => _timer.Tick -= OnTimerTick;

        public void Play()
        {
            AudioPositionEventArgs args = new AudioPositionEventArgs(Position);
            PlayRequested?.Invoke(this, args);
            Handler?.Invoke(nameof(Audio.PlayRequested), args);
        }

        public void Pause()
        {
            AudioPositionEventArgs args = new AudioPositionEventArgs(Position);
            PauseRequested?.Invoke(this, args);
            Handler?.Invoke(nameof(Audio.PauseRequested), args);
        }

        public void Stop()
        {
            AudioPositionEventArgs args = new AudioPositionEventArgs(Position);
            StopRequested?.Invoke(this, args);
            Handler?.Invoke(nameof(Audio.StopRequested), args);
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            UpdateStatus?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(Audio.UpdateStatus));
        }

        void SetTimeToEnd()
        {
            TimeToEnd = Duration - Position;
        }
    }
}
