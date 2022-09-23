namespace AudioDemos.Views;

public partial class BindToAudioPlayerPage : ContentPage
{
	public BindToAudioPlayerPage()
	{
		InitializeComponent();
	}

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        audio.Handler?.DisconnectHandler();
    }
}