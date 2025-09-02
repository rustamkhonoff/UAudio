using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

namespace UAudio
{
    public interface IAudioService
    {
        event Action<AudioRequest, AudioSource, CancellationToken> OnAudioPlayed;
        AudioMixerGroup SoundGroup { get; }
        AudioMixerGroup BackgroundGroup { get; }
        void Play(AudioRequest request, CancellationToken token = default);
        void PlayAt(AudioRequest request, Vector3 position, CancellationToken token = default);
        void ChangeBackground(AudioRequest request, bool restartIfSameClip = false, CancellationToken token = default);
        void StopBackground();
        void StopSounds();
        void SetSoundPauseState(bool state);
        void SetBackgroundPauseState(bool state);
        AudioSource PlayInBackground(AudioRequest request, AudioMixerGroup audioMixer = null, CancellationToken token = default);
        AudioData AudioDataFor(string key);
        void SetSoundVolume(float volume);
        void SetBackgroundVolume(float volume);
        void SetMasterVolume(float volume);
    }
}