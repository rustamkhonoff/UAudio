namespace UAudio
{
    public static class UAudioGlobal
    {
        public static bool Initialized { get; private set; }
        public static IAudioService Instance { get; internal set; }

        internal static void Initialize(IAudioService audioService)
        {
            Instance = audioService;
            Initialized = true;
        }

        static UAudioGlobal()
        {
            Instance = null;
            Initialized = false;
        }

        internal static void Dispose()
        {
            Initialized = false;
        }
    }
}