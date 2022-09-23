namespace AudioDemos.Controls
{
    public class AudioPositionEventArgs : EventArgs
    {
        public TimeSpan Position { get; private set; }

        public AudioPositionEventArgs(TimeSpan position)
        {
            Position = position;
        }
    }
}
