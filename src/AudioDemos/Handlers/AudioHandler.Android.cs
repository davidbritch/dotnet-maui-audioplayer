#nullable enable
using Microsoft.Maui.Handlers;
using AudioDemos.Controls;
using AudioDemos.Platforms.Android;

namespace AudioDemos.Handlers
{
    public partial class AudioHandler : ViewHandler<Audio, MauiAudioPlayer>
    {
        protected override MauiAudioPlayer CreatePlatformView() => new MauiAudioPlayer(Context, VirtualView);

        protected override void ConnectHandler(MauiAudioPlayer platformView)
        {
            base.ConnectHandler(platformView);

            // Perform any control setup here
        }

        protected override void DisconnectHandler(MauiAudioPlayer platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        public static void MapAreTransportControlsEnabled(AudioHandler handler, Audio audio)
        {
            handler.PlatformView?.UpdateTransportControlsEnabled();
        }

        public static void MapSource(AudioHandler handler, Audio audio)
        {
            handler.PlatformView?.UpdateSource();
        }

        public static void MapPosition(AudioHandler handler, Audio audio)
        {
            handler.PlatformView?.UpdatePosition();
        }

        public static void MapUpdateStatus(AudioHandler handler, Audio audio, object? args)
        {
            handler.PlatformView?.UpdateStatus();
        }

        public static void MapPlayRequested(AudioHandler handler, Audio audio, object? args)
        {
            if (args is not AudioPositionEventArgs)
                return;

            TimeSpan position = ((AudioPositionEventArgs)args).Position;
            handler.PlatformView?.PlayRequested(position);
        }

        public static void MapPauseRequested(AudioHandler handler, Audio audio, object? args)
        {
            if (args is not AudioPositionEventArgs)
                return;

            TimeSpan position = ((AudioPositionEventArgs)args).Position;
            handler.PlatformView?.PauseRequested(position);
        }

        public static void MapStopRequested(AudioHandler handler, Audio audio, object? args)
        {
            if (args is not AudioPositionEventArgs)
                return;

            TimeSpan position = ((AudioPositionEventArgs)args).Position;
            handler.PlatformView?.StopRequested(position);
        }
    }
}
