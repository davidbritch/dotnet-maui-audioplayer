namespace AudioDemos.Controls
{
    public interface IAudioController
    {
        AudioStatus Status { get; set; }
        TimeSpan Duration { get; set; }
    }
}
