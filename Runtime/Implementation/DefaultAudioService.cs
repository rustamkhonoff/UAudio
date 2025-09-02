using System;
using System.Collections;
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
        public event Action<AudioRequest, AudioSource, CancellationToken> OnAudioPlayed;

        private readonly IAudioConfiguration m_config;
        private readonly IAudioState m_state;
        private readonly Dictionary<string, AudioData> m_soundDatas;
        private readonly List<AudioSource> m_sources;

        private GameObject m_serviceParent;
        private AudioSource m_baseBackgroundSource;

        private bool m_configured;

        public DefaultAudioService(IAudioConfiguration config, IAudioState state)
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

        public void Play(AudioRequest request, CancellationToken token = default)
        {
            PlayAt(request, Vector3.zero, token);
        }

        public void PlayAt(AudioRequest request, Vector3 position, CancellationToken token = default)
        {
            EnsureServiceConfigured();

            AudioSource source = GetOrCreateSource();
            source.ApplyRequest(request, ClipForRequest);
            source.transform.position = position;
            source.PlayDelayed(request.Delay);
            source.outputAudioMixerGroup = SoundGroup;

            source.SetCancellationToken(token);

            PlayFade(ref request, source, token);
            OnAudioPlayed?.Invoke(request, source, token);
        }

        public void ChangeBackground(AudioRequest request, bool restartIfSameClip = false, CancellationToken token = default)
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

            PlayFade(ref request, source, token);
            OnAudioPlayed?.Invoke(request, source, token);
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
            if (state) m_sources.ForEach(a => a.Pause());
            else m_sources.ForEach(a => a.UnPause());
        }

        public void SetBackgroundPauseState(bool state)
        {
            if (state) m_baseBackgroundSource.Pause();
            else m_baseBackgroundSource.UnPause();
        }


        public AudioSource PlayInBackground(AudioRequest request, AudioMixerGroup audioMixer = null,
            CancellationToken token = default)
        {
            EnsureServiceConfigured();

            AudioSource source = GetOrCreateSource();
            source.ApplyRequest(request, ClipForRequest);
            source.PlayDelayed(request.Delay);
            source.outputAudioMixerGroup = BackgroundGroup;

            source.SetCancellationToken(token);

            PlayFade(ref request, source, token);
            OnAudioPlayed?.Invoke(request, source, token);

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

        private void PlayFade(ref AudioRequest req, AudioSource audioSource, CancellationToken cancellationToken)
        {
            if (req.Fade <= 0) return;
            if (req.Loop) return;

            float totalDuration = audioSource.clip.length / Math.Abs(audioSource.pitch);

            float left = totalDuration - req.Fade * 2f;
            if (left <= 0) return;

            float maxVolume = req.Volume;

            IEnumerator Fade(AudioRequest request)
            {
                for (float i = 0; i < 1f; i += Time.deltaTime / request.Fade)
                {
                    audioSource.volume = Mathf.Lerp(0f, maxVolume, i);
                    yield return null;
                }

                yield return new WaitForSeconds(left);

                for (float i = 0; i < 1f; i += Time.deltaTime / request.Fade)
                {
                    audioSource.volume = Mathf.Lerp(maxVolume, 0f, i);
                    yield return null;
                }
            }

            Coroutine coroutine = AudioServiceMonoHelper.Instance.StartCoroutine(Fade(req));
            cancellationToken.Register(() => AudioServiceMonoHelper.Instance.StopCoroutine(coroutine));
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