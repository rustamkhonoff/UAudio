using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace UAudio
{
    public class DefaultAudioService : IAudioService, IDisposable
    {
        private readonly Dictionary<string, AudioData> m_soundDatas;
        private readonly UAudioConfiguration m_config;
        private readonly IAudioState m_state;
        private readonly List<AudioSource> m_sources;

        private GameObject m_serviceParent;
        private AudioSource m_baseBackgroundSource;

        private bool m_configured;


        public DefaultAudioService(UAudioConfiguration config, IAudioState state)
        {
            m_state = state;
            m_config = config;
            m_sources = new List<AudioSource>();
            m_soundDatas = config.AudioDatas.ToDictionary(data => data.Key, data => data);

            state.BackgroundVolumeChanged += SetBackgroundVolume;
            state.SoundVolumeChanged += SetSoundVolume;
            state.RootVolumeChanged += SetSoundVolume;

            UAudioGlobal.Initialize(this);
        }

        public void Play(AudioRequest request, CancellationToken? token = null)
        {
            PlayAt(request, Vector3.zero, token);
        }

        public void PlayAt(AudioRequest request, Vector3 position, CancellationToken? token = null)
        {
            EnsureServiceConfigured();

            AudioSource source = GetOrCreateSource();
            source.ApplyRequest(request, ClipForRequest);
            source.transform.position = position;
            source.PlayDelayed(request.Delay);
            source.outputAudioMixerGroup = SoundGroup;

            source.SetCancellationToken(token);
        }

        public void ChangeBackground(AudioRequest request, bool restartIfSameClip = false, CancellationToken? token = null)
        {
            EnsureServiceConfigured();
            AudioSource source = m_baseBackgroundSource;
            AudioClip clip = ClipForRequest(request);
            AudioClip currentClip = source.clip;

            if (currentClip != null && currentClip == clip && restartIfSameClip && source.isPlaying)
            {
                source.ApplyRequest(request, ClipForRequest);
                source.PlayDelayed(request.Delay);
            }
            else if (currentClip == null || currentClip != clip || !source.isPlaying)
            {
                source.ApplyRequest(request, ClipForRequest);
                source.PlayDelayed(request.Delay);
            }

            source.SetCancellationToken(token);
        }

        public void StopBackground()
        {
            m_baseBackgroundSource.Stop();
        }

        public void StopSounds()
        {
            m_sources.ForEach(a => a.Stop());
        }

        public void SetSoundPauseState(bool state)
        {
            Action<AudioSource> toDo = state ? a => a.Pause() : a => a.UnPause();
            m_sources.ForEach(toDo);
        }

        public void SetBackgroundPauseState(bool state)
        {
            Action<AudioSource> toDo = state ? a => a.Pause() : a => a.UnPause();
            toDo.Invoke(m_baseBackgroundSource);
        }


        public AudioSource PlayInBackground(AudioRequest request, AudioMixerGroup audioMixer = null,
            CancellationToken? token = null)
        {
            EnsureServiceConfigured();

            AudioSource source = GetOrCreateSource();
            source.ApplyRequest(request, ClipForRequest);
            source.PlayDelayed(request.Delay);
            source.outputAudioMixerGroup = BackgroundGroup;

            source.SetCancellationToken(token);

            return source;
        }

        public AudioData AudioDataFor(string key)
        {
            return m_soundDatas[key];
        }


        public void SetSoundVolume(float volume)
        {
            EnsureServiceConfigured();

            m_config.SoundGroup.audioMixer.SetFloat(DefaultSoundVolumeKey, volume.ToDb());
        }

        public void SetBackgroundVolume(float volume)
        {
            EnsureServiceConfigured();

            m_config.BackgroundGroup.audioMixer.SetFloat(DefaultBackgroundVolumeKey, volume.ToDb());
        }

        public void SetMasterVolume(float volume)
        {
            EnsureServiceConfigured();

            m_config.MasterGroup.audioMixer.SetFloat(DefaultMasterVolumeKey, volume.ToDb());
        }

        private AudioClip ClipForRequest(AudioRequest audioRequest)
        {
            if (audioRequest.ByKey
                && audioRequest.Key != null
                && !string.IsNullOrEmpty(audioRequest.Key)
                && m_soundDatas.TryGetValue(audioRequest.Key, out AudioData data))
                return data.Clip;
            else if (audioRequest.Clip != null)
                return audioRequest.Clip;

            Debug.LogError($"There is no Audio Clip for request {audioRequest}");

            return null;
        }

        private AudioSource GetOrCreateSource()
        {
            AudioSource source = m_sources.Find(a => !a.isPlaying);
            if (source == null)
            {
                GameObject newChild = new($"Audio Source {m_sources.Count}");
                newChild.transform.parent = m_serviceParent.transform;
                source = newChild.AddComponent<AudioSource>();
                m_sources.Add(source);
            }

            ResetAudioSource(source);
            return source;
        }

        private void ResetAudioSource(AudioSource source)
        {
            source.clip = null;
            source.volume = 0f;
            source.loop = false;
            source.pitch = 1f;
            source.playOnAwake = false;
            source.transform.position = Vector3.zero;
            source.outputAudioMixerGroup = null;
        }

        private void EnsureServiceConfigured()
        {
            if (m_configured) return;
            m_configured = true;

            m_serviceParent = new GameObject("AudioService");
            Object.DontDestroyOnLoad(m_serviceParent);

            m_baseBackgroundSource = new GameObject("AudioService BackgroundSource").AddComponent<AudioSource>();
            ResetAudioSource(m_baseBackgroundSource);

            m_baseBackgroundSource.transform.parent = m_serviceParent.transform;
            m_baseBackgroundSource.outputAudioMixerGroup = BackgroundGroup;

            SetBackgroundVolume(m_state.BackgroundVolume);
            SetSoundVolume(m_state.SoundVolume);
            SetMasterVolume(m_state.RootVolume);
        }

        public void Dispose()
        {
            m_state.BackgroundVolumeChanged -= SetBackgroundVolume;
            m_state.SoundVolumeChanged -= SetSoundVolume;
            m_state.RootVolumeChanged -= SetSoundVolume;

            m_sources.Clear();
            Object.Destroy(m_serviceParent);

            UAudioGlobal.Dispose();
        }

        public AudioMixerGroup SoundGroup => m_config.SoundGroup;
        public AudioMixerGroup BackgroundGroup => m_config.BackgroundGroup;

        private const string DefaultSoundVolumeKey = "soundVolume";
        private const string DefaultBackgroundVolumeKey = "bgVolume";
        private const string DefaultMasterVolumeKey = "masterVolume";
    }
}