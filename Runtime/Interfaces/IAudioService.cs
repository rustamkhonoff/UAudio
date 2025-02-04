using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

namespace UAudio
{
    public interface IAudioService
    {
        AudioMixerGroup SoundGroup { get; }
        AudioMixerGroup BackgroundGroup { get; }
        void Play(AudioRequest request, CancellationToken? token = null);
        void PlayAt(AudioRequest request, Vector3 position, CancellationToken? token = null);
        void ChangeBackground(AudioRequest request, bool restartIfSameClip = false, CancellationToken? token = null);
        void StopBackground();
        void StopSounds();
        void SetSoundPauseState(bool state);
        void SetBackgroundPauseState(bool state);
        AudioSource PlayInBackground(AudioRequest request, AudioMixerGroup audioMixer = null, CancellationToken? token = null);
        AudioData AudioDataFor(string key);
        void SetSoundVolume(float volume);
        void SetBackgroundVolume(float volume);
        void SetMasterVolume(float volume);
    }
}