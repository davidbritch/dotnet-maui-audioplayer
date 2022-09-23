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
                    { DevicePlatform.Android, new [] { "*.mp3", ".3gp", ".mp4", ".m4a", ".aac", ".ts", ".amr", ".flac", ".mid", ".xmf", ".mxmf", ".rtttl", ".rtx", ".ota", ".imy", ".mkv", ".ogg", ".wav" } },
                    { DevicePlatform.iOS, new[] { "*.mp3", "*.aac", "*.aifc", "*.au", "*.aiff", "*.mp2", "*.3gp", "*.ac3" } }
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
