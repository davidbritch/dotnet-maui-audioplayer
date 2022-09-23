using System.ComponentModel;

namespace AudioDemos.Controls
{
    public class AudioSourceConverter : TypeConverter, IExtendedTypeConverter
    {
        object IExtendedTypeConverter.ConvertFromInvariantString(string value, IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Uri uri;
                return Uri.TryCreate(value, UriKind.Absolute, out uri) && uri.Scheme != "file" ?
                    AudioSource.FromUri(value) : AudioSource.FromResource(value);
            }
            throw new InvalidOperationException("Cannot convert null or whitespace to AudioSource.");
        }
    }
}
