using System;

namespace UAudio
{
    public class DefaultAudioState : IAudioState
    {
        public event Action<float> BackgroundVolumeChanged;
        public event Action<float> SoundVolumeChanged;
        public event Action<float> RootVolumeChanged;
        public float SoundVolume => 1f;
        public float RootVolume => 1f;
        public float BackgroundVolume => 1f;
    }
}