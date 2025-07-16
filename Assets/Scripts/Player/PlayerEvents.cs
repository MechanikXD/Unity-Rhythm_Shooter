using System;
using Player.Statistics.Score.Rank;

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
        
        public static event Action<int> DamageCalculated;
        public static event Action DamageDealt;
        public static event Action AttackFailed;

        public static void OnDamageCalculated(ref int damage) => DamageCalculated?.Invoke(damage);
        public static void OnAttackFailed() => AttackFailed?.Invoke();
        public static void OnDamageDealt() => DamageDealt?.Invoke();

        
        public static event Action<long> ScoreChanged;
        public static event Action<int> ComboCountChanged;
        public static event Action<RankLetter> RankIncreased;
        public static event Action<RankLetter> RankDecreased;

        public static void OnScoreChanged(long newScore) => ScoreChanged?.Invoke(newScore);
        public static void OnComboCountChanged(int newComboCount) => ComboCountChanged?.Invoke(newComboCount);
        public static void OnRankIncreased(RankLetter newRank) => RankIncreased?.Invoke(newRank);
        public static void OnRankDecreased(RankLetter newRank) => RankDecreased?.Invoke(newRank);
    }
}