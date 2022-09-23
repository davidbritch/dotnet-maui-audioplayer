namespace AudioDemos.Controls
{
    public class AudioInfo
    {
        public string DisplayName { get; set; }
        public AudioSource AudioSource { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
