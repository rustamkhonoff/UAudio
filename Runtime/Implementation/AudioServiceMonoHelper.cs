using UnityEngine;

namespace UAudio
{
    internal class AudioServiceMonoHelper : MonoBehaviour
    {
        private static AudioServiceMonoHelper _instance;

        public static AudioServiceMonoHelper Instance
        {
            get
            {
                if (_instance != null) return _instance;

                AudioServiceMonoHelper helper = new GameObject(nameof(AudioServiceMonoHelper)).AddComponent<AudioServiceMonoHelper>();
                _instance = helper;
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null) Destroy(this);
        }
    }
}