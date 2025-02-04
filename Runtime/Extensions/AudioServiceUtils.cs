using System;
using UnityEngine;

namespace UAudio
{
    public static class AudioServiceExtensions
    {
        public static AudioSource ApplyRequest(this AudioSource audioSource, AudioRequest request, Func<AudioRequest, AudioClip> clipFunc)
        {
            audioSource.clip = clipFunc(request);
            audioSource.volume = request.Volume;
            audioSource.pitch = request.Pitch;
            audioSource.loop = request.Loop;
            return audioSource;
        }

        public static float ToDb(this float t)
        {
            return t <= 0.0001f ? -80f : 20f * Mathf.Log10(t);
        }
    }
}