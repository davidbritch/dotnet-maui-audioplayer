using Microsoft.UI.Xaml.Controls;
using AudioDemos.Controls;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Grid = Microsoft.UI.Xaml.Controls.Grid;

namespace AudioDemos.Platforms.Windows
{
    public class MauiAudioPlayer : Grid, IDisposable
    {
        MediaPlayerElement _mediaPlayerElement;
        Audio _audio;
        bool _isMediaPlayerAttached;

        public MauiAudioPlayer(Audio audio)
        {
            _audio = audio;
            _mediaPlayerElement = new MediaPlayerElement();
            this.Children.Add(_mediaPlayerElement);
        }

        public void Dispose()
        {
            if (_isMediaPlayerAttached)
            {
                _mediaPlayerElement.MediaPlayer.MediaOpened -= OnMediaPlayerMediaOpened;
                _mediaPlayerElement.MediaPlayer.Dispose();
            }
            _mediaPlayerElement = null;
        }

        public void UpdateTransportControlsEnabled()
        {
            _mediaPlayerElement.AreTransportControlsEnabled = _audio.AreTransportControlsEnabled;
        }

        public async void UpdateSource()
        {
            bool hasSetSource = false;

            if (_audio.Source is UriAudioSource)
            {
                string uri = (_audio.Source as UriAudioSource).Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(uri));
                    hasSetSource = true;
                }
            }
            else if (_audio.Source is FileAudioSource)
            {
                string filename = (_audio.Source as FileAudioSource).File;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filename);
                    _mediaPlayerElement.Source = MediaSource.CreateFromStorageFile(storageFile);
                    hasSetSource = true;
                }
            }
            else if (_audio.Source is ResourceAudioSource)
            {
                string path = "ms-appx:///" + (_audio.Source as ResourceAudioSource).Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(path));
                    hasSetSource = true;
                }
            }

            if (hasSetSource && !_isMediaPlayerAttached)
            {
                _isMediaPlayerAttached = true;
                _mediaPlayerElement.MediaPlayer.MediaOpened += OnMediaPlayerMediaOpened;
            }

            if (hasSetSource && _audio.AutoPlay)
            {
                _mediaPlayerElement.AutoPlay = true;
            }
        }

        public void UpdatePosition()
        {
            if (_isMediaPlayerAttached)
            {
                if (Math.Abs((_mediaPlayerElement.MediaPlayer.Position - _audio.Position).TotalSeconds) > 1)
                {
                    _mediaPlayerElement.MediaPlayer.Position = _audio.Position;
                }
            }
        }

        public void UpdateStatus()
        {
            if (_isMediaPlayerAttached)
            {
                AudioStatus status = AudioStatus.NotReady;

                switch (_mediaPlayerElement.MediaPlayer.CurrentState)
                {
                    case MediaPlayerState.Playing:
                        status = AudioStatus.Playing;
                        break;
                    case MediaPlayerState.Paused:
                    case MediaPlayerState.Stopped:
                        status = AudioStatus.Paused;
                        break;
                }

                ((IAudioController)_audio).Status = status;
                _audio.Position = _mediaPlayerElement.MediaPlayer.Position;
            }
        }

        public void PlayRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                _mediaPlayerElement.MediaPlayer.Play();
                System.Diagnostics.Debug.WriteLine($"Audio playback from {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        public void PauseRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                _mediaPlayerElement.MediaPlayer.Pause();
                System.Diagnostics.Debug.WriteLine($"Audio paused at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        public void StopRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                // There's no Stop method so pause the audio and reset its position
                _mediaPlayerElement.MediaPlayer.Pause();
                _mediaPlayerElement.MediaPlayer.Position = TimeSpan.Zero;
                System.Diagnostics.Debug.WriteLine($"Audio stopped at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        void OnMediaPlayerMediaOpened(MediaPlayer sender, object args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((IAudioController)_audio).Duration = _mediaPlayerElement.MediaPlayer.NaturalDuration;
            });
        }
    }
}
