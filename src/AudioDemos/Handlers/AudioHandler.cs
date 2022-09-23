#if IOS || MACCATALYST
using PlatformView = AudioDemos.Platforms.MaciOS.MauiAudioPlayer;
#elif ANDROID
using PlatformView = AudioDemos.Platforms.Android.MauiAudioPlayer;
#elif WINDOWS
using PlatformView = AudioDemos.Platforms.Windows.MauiAudioPlayer;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0 && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif
using AudioDemos.Controls;
using Microsoft.Maui.Handlers;

namespace AudioDemos.Handlers
{
    public partial class AudioHandler
    {
        public static IPropertyMapper<Audio, AudioHandler> PropertyMapper = new PropertyMapper<Audio, AudioHandler>(ViewHandler.ViewMapper)
        {
            [nameof(Audio.AreTransportControlsEnabled)] = MapAreTransportControlsEnabled,
            [nameof(Audio.Source)] = MapSource,
            [nameof(Audio.Position)] = MapPosition
        };

        public static CommandMapper<Audio, AudioHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(Audio.UpdateStatus)] = MapUpdateStatus,
            [nameof(Audio.PlayRequested)] = MapPlayRequested,
            [nameof(Audio.PauseRequested)] = MapPauseRequested,
            [nameof(Audio.StopRequested)] = MapStopRequested
        };

        public AudioHandler() : base(PropertyMapper, CommandMapper)
        {
        }
    }
}
