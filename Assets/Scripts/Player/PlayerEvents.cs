using System;

namespace Player {
    public static class PlayerEvents {
        public static event Action Jumped;
        public static event Action BecomeGrounded;
        public static void OnBecomeGrounded() => BecomeGrounded?.Invoke();   
        public static void OnJumpedEvent() => Jumped?.Invoke();     
        
        public static event Action Dashed;
        public static event Action ExitedDash;
        public static void OnDashedEvent() => Dashed?.Invoke(); 
        public static void OnExitedDashEvent() => ExitedDash?.Invoke();
        
        public static event Action StartWalking;
        public static event Action StoppedWalking;
        public static void OnStartWalkingEvent() => StartWalking?.Invoke();
        public static void OnStoppedWalkingEvent() => StoppedWalking?.Invoke();
        
        public static event Action DamageDealt;
        public static event Action AttackFailed;

        public static void OnAttackFailed() => AttackFailed?.Invoke();
        public static void OnDamageDealt() => DamageDealt?.Invoke();
        
        public static event Action<long> ScoreChanged;
        public static event Action<int> ComboCountChanged;
        public static event Action RankIncreased;
        public static event Action RankDecreased;
        public static event Action<float> RankPerformanceChanged;

        public static void OnScoreChanged(long newScore) => ScoreChanged?.Invoke(newScore);
        public static void OnComboCountChanged(int newComboCount) => ComboCountChanged?.Invoke(newComboCount);
        public static void OnRankIncreased() => RankIncreased?.Invoke();
        public static void OnRankDecreased() => RankDecreased?.Invoke();
        public static void OnRankPerformanceChanged(float newRankPerformance) =>
            RankPerformanceChanged?.Invoke(newRankPerformance);
    }
}