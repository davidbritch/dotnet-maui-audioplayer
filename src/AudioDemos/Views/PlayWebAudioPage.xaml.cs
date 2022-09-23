namespace AudioDemos.Views;

public partial class PlayWebAudioPage : ContentPage
{
	public PlayWebAudioPage()
	{
		InitializeComponent();
	}

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        audio.Handler?.DisconnectHandler();
    }
}