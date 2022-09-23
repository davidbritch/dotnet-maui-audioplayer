using System.ComponentModel;

namespace AudioDemos.Controls
{
    
    [TypeConverter(typeof(AudioSourceConverter))]
    public abstract class AudioSource : Element
    {
        public static AudioSource FromUri(string uri)
        {
            return new UriAudioSource { Uri = uri };
        }

        public static AudioSource FromFile(string file)
        {
            return new FileAudioSource { File = file };
        }

        public static AudioSource FromResource(string path)
        {
            return new ResourceAudioSource { Path = path };
        }
    }
}
