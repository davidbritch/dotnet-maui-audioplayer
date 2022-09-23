namespace AudioDemos.Views;

public partial class PlayAudioResourcePage : ContentPage
{
	public PlayAudioResourcePage()
	{
		InitializeComponent();
	}

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        audio.Handler?.DisconnectHandler();
    }
}