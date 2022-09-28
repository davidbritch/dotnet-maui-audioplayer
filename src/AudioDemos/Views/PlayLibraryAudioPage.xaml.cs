using AudioDemos.Controls;

namespace AudioDemos.Views;

public partial class PlayLibraryAudioPage : ContentPage
{
	public PlayLibraryAudioPage()
	{
		InitializeComponent();
	}

    async void OnShowAudioLibraryClicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        button.IsEnabled = false;

        var pickedAudio = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select audio file",
            FileTypes = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new [] { "*.mp3", "*.m4a" } },
                    { DevicePlatform.Android, new [] { "audio/*" } },
                    { DevicePlatform.iOS, new[] { "public.audio" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.audio" } }
                })
        });

        if (!string.IsNullOrWhiteSpace(pickedAudio?.FileName))
        {
            audio.Source = new FileAudioSource
            {
                File = pickedAudio.FullPath
            };
        }

        button.IsEnabled = true;
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        audio.Handler?.DisconnectHandler();
    }
}
