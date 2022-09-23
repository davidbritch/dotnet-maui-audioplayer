using AudioDemos.Controls;

namespace AudioDemos.Views;

public partial class SelectWebAudioPage : ContentPage
{
	public SelectWebAudioPage()
	{
		InitializeComponent();
	}

    void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string key = ((string)e.CurrentSelection.FirstOrDefault()).Replace(" ", "").Replace("'", "");
        audio.Source = (UriAudioSource)Application.Current.Resources[key];
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        audio.Handler?.DisconnectHandler();
    }
}