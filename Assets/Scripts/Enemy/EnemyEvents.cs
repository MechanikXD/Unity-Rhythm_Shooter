using System;

namespace Enemy {
    public static class EnemyEvents {
        public static event Action EnemyDefeated;
        public static event Action TargetDefeated;
        public static event Action NormalDefeated;
        
        public static void OnEnemyDefeated() => EnemyDefeated?.Invoke();
        public static void OnTargetDefeated() => TargetDefeated?.Invoke();
        public static void OnNormalDefeated() => NormalDefeated?.Invoke();
    }
}