using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UAudio
{
    [Serializable]
    public struct AudioRequest
    {
        public bool ByKey;
        [CanBeNull] public string Key;
        [CanBeNull] public AudioClip Clip;
        public float Volume;
        public float Pitch;
        public bool Loop;
        public float Delay;

        public AudioRequest(string key, float volume = 1f, float pitch = 1f, bool loop = false, float delay = 0f)
        {
            Key = key;
            Clip = null;
            Pitch = pitch;
            Volume = volume;
            Loop = loop;
            Delay = delay;
            ByKey = true;
        }

        public AudioRequest(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false, float delay = 0f)
        {
            Key = null;
            Clip = clip;
            Pitch = pitch;
            Volume = volume;
            Loop = loop;
            Delay = delay;
            ByKey = false;
        }

        public override string ToString()
        {
            return
                $"{nameof(Key)}: {Key}, {nameof(Clip)}: {Clip}, {nameof(Volume)}: {Volume}, {nameof(Pitch)}: {Pitch}, {nameof(Loop)}: {Loop}, {nameof(Delay)}: {Delay}";
        }

        public static implicit operator AudioRequest(AudioClip clip) => new(clip);
        public static implicit operator AudioRequest(string key) => new(key);
    }
}