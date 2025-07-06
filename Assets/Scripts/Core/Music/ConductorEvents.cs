using System;

namespace Core.Music {
    public static class ConductorEvents {
        public static event Action NextBeatEvent;

        public static event Action BeatMissedEvent;
        public static event Action PerfectBeatHitEvent;
        public static event Action GoodBeatHitEvent;
        
        public static void OnOnNextBeat() => NextBeatEvent?.Invoke();

        public static void OnBeatMissed() => BeatMissedEvent?.Invoke();
        public static void OnPerfectBeatHit() => PerfectBeatHitEvent?.Invoke();
        public static void OnGoodBeatHit() => GoodBeatHitEvent?.Invoke();
    }
}