using UnityEngine;

namespace UAudio
{
    public static class AudioRequestExtensions
    {
        public static AudioRequest ToAudioRequest(this string key, float volume = 1f, float pitch = 1f, bool loop = false, float delay = 0f) =>
            new(key, volume, pitch, loop, delay);

        public static AudioRequest ToAudioRequest(this AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false, float delay = 0f) =>
            new(clip, volume, pitch, loop, delay);

        public static AudioRequest WithRandomPitch(this ref AudioRequest request, float min = 1f, float max = 1.25f)
        {
            float pitch = Random.Range(min, max);
            request.Pitch = pitch;
            return request;
        }
    }
}