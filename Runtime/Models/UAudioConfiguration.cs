using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UAudio
{
    [CreateAssetMenu(menuName = "Services/UAudio/Create UAudioConfiguration", fileName = "UAudioConfiguration", order = 0)]
    public class UAudioConfiguration : ScriptableObject, IAudioConfiguration
    {
        [field: SerializeField] public AudioMixerGroup MasterGroup { get; private set; }
        [field: SerializeField] public AudioMixerGroup SoundGroup { get; private set; }
        [field: SerializeField] public AudioMixerGroup BackgroundGroup { get; private set; }
        [field: SerializeField] public List<AudioData> AudioDatas { get; private set; }
    }
}