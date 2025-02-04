using System.Collections.Generic;
using UnityEngine.Audio;

namespace UAudio
{
    public interface IAudioConfiguration
    {
        AudioMixerGroup MasterGroup { get; }
        AudioMixerGroup SoundGroup { get; }
        AudioMixerGroup BackgroundGroup { get; }
        List<AudioData> AudioDatas { get; }
    }
}