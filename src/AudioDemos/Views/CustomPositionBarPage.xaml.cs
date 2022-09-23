using AudioDemos.Controls;

namespace AudioDemos.Views;

public partial class CustomPositionBarPage : ContentPage
{
	public CustomPositionBarPage()
	{
		InitializeComponent();
	}


    void OnPlayPauseButtonClicked(object sender, EventArgs args)
    {
        if (audio.Status == AudioStatus.Playing)
        {
            audio.Pause();
        }
        else if (audio.Status == AudioStatus.Paused)
        {
            audio.Play();
        }
    }

    void OnStopButtonClicked(object sender, EventArgs args)
    {
        audio.Stop();
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        audio.Handler?.DisconnectHandler();
    }
}
