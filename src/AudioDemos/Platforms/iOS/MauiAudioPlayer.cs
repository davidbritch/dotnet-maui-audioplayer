using System.Diagnostics;
using AudioDemos.Controls;
using AVFoundation;
using AVKit;
using CoreMedia;
using Foundation;
using UIKit;

namespace AudioDemos.Platforms.MaciOS
{
    public class MauiAudioPlayer : UIView
    {
        AVPlayer _player;
        AVPlayerItem _playerItem;
        AVPlayerViewController _playerViewController;
        Audio _audio;

        public MauiAudioPlayer(Audio audio)
        {
            _audio = audio;

            // Create AVPlayerViewController
            _playerViewController = new AVPlayerViewController();

            // Set Player property to AVPlayer
            _player = new AVPlayer();
            _playerViewController.Player = _player;

            // Use the View from the controller as the native control
            _playerViewController.View.Frame = this.Bounds;
            AddSubview(_playerViewController.View);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_player != null)
                {
                    _player.ReplaceCurrentItemWithPlayerItem(null);
                    _player.Dispose();
                }
                if (_playerViewController != null)
                    _playerViewController.Dispose();

                _audio = null;
            }

            base.Dispose(disposing);
        }

        public void UpdateTransportControlsEnabled()
        {
            _playerViewController.ShowsPlaybackControls = _audio.AreTransportControlsEnabled;
            if (_playerViewController.ShowsPlaybackControls)
                AddContentOverlayImage();
        }

        public void UpdateSource()
        {
            AVAsset asset = null;

            if (_audio.Source is UriAudioSource)
            {
                string uri = (_audio.Source as UriAudioSource).Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                    asset = AVAsset.FromUrl(new NSUrl(uri));
            }
            else if (_audio.Source is FileAudioSource)
            {
                string uri = (_audio.Source as FileAudioSource).File;
                if (!string.IsNullOrWhiteSpace(uri))
                    asset = AVAsset.FromUrl(new NSUrl(uri));
            }
            else if (_audio.Source is ResourceAudioSource)
            {
                string path = (_audio.Source as ResourceAudioSource).Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string directory = Path.GetDirectoryName(path);
                    string filename = Path.GetFileNameWithoutExtension(path);
                    string extension = Path.GetExtension(path).Substring(1);
                    NSUrl url = NSBundle.MainBundle.GetUrlForResource(filename, extension, directory);
                    asset = AVAsset.FromUrl(url);
                }
            }

            if (asset != null)
                _playerItem = new AVPlayerItem(asset);
            else
                _playerItem = null;

            _player.ReplaceCurrentItemWithPlayerItem(_playerItem);
            if (_playerItem != null && _audio.AutoPlay)
            {
                _player.Play();
            }
        }

        public void UpdatePosition()
        {
            TimeSpan controlPosition = ConvertTime(_player.CurrentTime);
            if (Math.Abs((controlPosition - _audio.Position).TotalSeconds) > 1)
            {
                _player.Seek(CMTime.FromSeconds(_audio.Position.TotalSeconds, 1));
            }
        }

        TimeSpan ConvertTime(CMTime cmTime)
        {
            return TimeSpan.FromSeconds(Double.IsNaN(cmTime.Seconds) ? 0 : cmTime.Seconds);
        }

        public void UpdateStatus()
        {
            AudioStatus audioStatus = AudioStatus.NotReady;

            switch (_player.Status)
            {
                case AVPlayerStatus.ReadyToPlay:
                    switch (_player.TimeControlStatus)
                    {
                        case AVPlayerTimeControlStatus.Playing:
                            audioStatus = AudioStatus.Playing;
                            break;

                        case AVPlayerTimeControlStatus.Paused:
                            audioStatus = AudioStatus.Paused;
                            break;
                    }
                    break;
            }
            ((IAudioController)_audio).Status = audioStatus;

            if (_playerItem != null)
            {
                ((IAudioController)_audio).Duration = ConvertTime(_playerItem.Duration);
                _audio.Position = ConvertTime(_playerItem.CurrentTime);
            }
        }

        public void PlayRequested(TimeSpan position)
        {
            _player.Play();
            Debug.WriteLine($"Audio playback from {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void PauseRequested(TimeSpan position)
        {
            _player.Pause();
            Debug.WriteLine($"Audio paused at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void StopRequested(TimeSpan position)
        {
            _player.Pause();
            _player.Seek(new CMTime(0, 1));
            Debug.WriteLine($"Audio stopped at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        void AddContentOverlayImage()
        {
            UIImageView imageView = new UIImageView();
            imageView.Image = UIImage.FromBundle("dotnet_bot.png");
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            _playerViewController.ContentOverlayView.AddSubview(imageView);

            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.BottomAnchor.ConstraintEqualTo(_playerViewController.ContentOverlayView.BottomAnchor).Active = true;
            imageView.TopAnchor.ConstraintEqualTo(_playerViewController.ContentOverlayView.TopAnchor).Active = true;
            imageView.LeadingAnchor.ConstraintEqualTo(_playerViewController.ContentOverlayView.LeadingAnchor).Active = true;
            imageView.TrailingAnchor.ConstraintEqualTo(_playerViewController.ContentOverlayView.TrailingAnchor).Active = true;
        }
    }
}
