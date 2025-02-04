using System.Threading;
using UnityEngine;

namespace UAudio
{
    public static class AudioSourceExtensions
    {
        private class AudioCancellationHolder : MonoBehaviour
        {
            public CancellationTokenRegistration CancellationRegistration { get; set; }

            private void OnDestroy()
            {
                CancellationRegistration.Dispose();
            }
        }

        public static void SetCancellationToken(this AudioSource source, CancellationToken? token)
        {
            AudioCancellationHolder audioCancellationHolder =
                source.GetComponent<AudioCancellationHolder>()
                ?? source.gameObject.AddComponent<AudioCancellationHolder>();

            audioCancellationHolder.CancellationRegistration.Dispose();

            if (!token.HasValue)
                return;
            audioCancellationHolder.CancellationRegistration = token.Value.Register(() =>
            {
                if (source.isPlaying) source.Stop();
            });
        }
    }
}