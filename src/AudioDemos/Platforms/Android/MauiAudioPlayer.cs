using Android.Content;
using Android.Media;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AudioDemos.Controls;
using Color = Android.Graphics.Color;
using Uri = Android.Net.Uri;

namespace AudioDemos.Platforms.Android
{
    public class MauiAudioPlayer : CoordinatorLayout, MediaPlayer.IOnPreparedListener, MediaController.IMediaPlayerControl
    {
        readonly MediaPlayer _mediaPlayer;
        MediaController _mediaController;
        bool _isPrepared;
        Context _context;
        Audio _audio;

        public MauiAudioPlayer(Context context, Audio audio, double volume = 1, double balance = 0) : base(context)
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetOnPreparedListener(this);
            _context = context;
            _audio = audio;

            SetVolume(volume, balance);
            SetBackgroundColor(Color.Black);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mediaPlayer.Release();
                _mediaPlayer.Dispose();
            }
            base.Dispose(disposing);
        }

        public void UpdateTransportControlsEnabled()
        {
            if (_audio.AreTransportControlsEnabled)
            {
                _mediaController = new MediaController(_context);
                _mediaController.SetMediaPlayer(this);
                _mediaController.SetAnchorView(this);
            }
            else
            {
                if (_mediaController != null)
                {
                    _mediaController.SetMediaPlayer(null);
                    _mediaController = null;
                }
            }
        }

        public void UpdateSource()
        {
            _isPrepared = false;
            bool hasSetSource = false;

            if (_audio.Source is UriAudioSource)
            {
                string uri = (_audio.Source as UriAudioSource).Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _mediaPlayer.SetDataSource(uri);
                    _mediaPlayer.PrepareAsync(); // Required for buffering
                    hasSetSource = true;
                }
            }
            else if (_audio.Source is FileAudioSource)
            {
                string filename = (_audio.Source as FileAudioSource).File;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    _mediaPlayer.SetDataSource(filename);
                    _mediaPlayer.Start();
                    hasSetSource = true;
                }
            }
            else if (_audio.Source is ResourceAudioSource)
            {
                string package = Context.PackageName;
                string path = (_audio.Source as ResourceAudioSource).Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string assetFilePath = "content://" + package + "/" + path;
                    _mediaPlayer.SetDataSource(Context, Uri.Parse(assetFilePath));
                    _mediaPlayer.PrepareAsync();
                    hasSetSource = true;
                }
            }

            if (hasSetSource && _audio.AutoPlay)
            {
                _mediaPlayer.Start();
            }
        }

        public void UpdatePosition()
        {
            if (Math.Abs(_mediaPlayer.CurrentPosition - _audio.Position.TotalMilliseconds) > 1000)
            {
                _mediaPlayer.SeekTo((int)_audio.Position.TotalMilliseconds);
            }
        }

        public void UpdateStatus()
        {
            AudioStatus status = AudioStatus.NotReady;

            if (_isPrepared)
            {
                status = _mediaPlayer.IsPlaying ? AudioStatus.Playing : AudioStatus.Paused;
            }

            ((IAudioController)_audio).Status = status;

            // Set Position property
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(_mediaPlayer.CurrentPosition);
            _audio.Position = timeSpan;
        }

        public void PlayRequested(TimeSpan position)
        {
            _mediaPlayer.Start();
            System.Diagnostics.Debug.WriteLine($"Audio playback from {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void PauseRequested(TimeSpan position)
        {
            _mediaPlayer.Pause();
            System.Diagnostics.Debug.WriteLine($"Audio paused at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void StopRequested(TimeSpan position)
        {
            // MediaPlayer.Stop stops the audio but playback can't be started until Prepare or PrepareAsync are called.
            _mediaPlayer.Pause();
            _mediaPlayer.SeekTo(0);
            System.Diagnostics.Debug.WriteLine($"Audio stopped at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        void SetVolume(double volume, double balance)
        {
            volume = Math.Clamp(volume, 0, 1);
            balance = Math.Clamp(balance, -1, 1);
            double left = Math.Cos((Math.PI * (balance + 1)) / 4) * volume;
            double right = Math.Sin((Math.PI * (balance + 1)) / 4) * volume;
            _mediaPlayer.SetVolume((float)left, (float)right);
        }

        // Called when PrepareAsync completes, meaning the audio can be played
        void MediaPlayer.IOnPreparedListener.OnPrepared(MediaPlayer mp)
        {
            _isPrepared = true;
            ((IAudioController)_audio).Duration = TimeSpan.FromMilliseconds(_mediaPlayer.Duration);

            if (_audio.AreTransportControlsEnabled)
                _mediaController.Show();
            mp.Start();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (_audio.AreTransportControlsEnabled)
                _mediaController.Show();
            return false;
        }

        #region IMediaPlayerControl

        public int AudioSessionId => 1;
        public int BufferPercentage => 0;
        public int CurrentPosition => _mediaPlayer.CurrentPosition;
        public int Duration => _mediaPlayer.Duration;
        public bool IsPlaying => _mediaPlayer.IsPlaying;

        public bool CanPause()
        {
            return true;
        }

        public bool CanSeekBackward()
        {
            return true;
        }

        public bool CanSeekForward()
        {
            return true;
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
        }

        public void SeekTo(int pos)
        {
            _mediaPlayer.SeekTo(pos);
        }

        public void Start()
        {
            _mediaPlayer.Start();
        }

        #endregion
    }
}
