using System;

namespace Core.Music {
    public static class ConductorEvents {
        public static event Action NextBeatEvent;

        public static void OnOnNextBeat() => NextBeatEvent?.Invoke();
    }
}