using System.Threading;
using UnityEngine;

namespace UAudio.UI
{
    public class AudioInvocator : MonoBehaviour
    {
        [SerializeField] private AudioRequest _request = new("");
        [SerializeField] private bool _objectDestroyAsCancellation = true;

        public void Play()
        {
            CancellationToken token = _objectDestroyAsCancellation ? destroyCancellationToken : default;

            if (UAudioGlobal.Initialized) UAudioGlobal.Instance?.Play(_request, token);
            else Debug.LogWarning("IAudioService instance not initialized in UAudioGlobal");
        }
    }
}