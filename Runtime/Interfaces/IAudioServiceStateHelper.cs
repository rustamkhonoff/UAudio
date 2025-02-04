using System;

namespace UAudio
{
    public interface IAudioState 
    {
        event Action<float> BackgroundVolumeChanged;
        event Action<float> SoundVolumeChanged;
        event Action<float> RootVolumeChanged;
        float SoundVolume { get; }
        float RootVolume { get; }
        float BackgroundVolume { get; }
    }
}