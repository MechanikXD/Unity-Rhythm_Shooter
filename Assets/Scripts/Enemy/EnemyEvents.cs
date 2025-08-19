using System;
using Enemy.Base;

namespace Enemy {
    public static class EnemyEvents {
        public static event Action<EnemyBase> EnemyDefeated;
        public static event Action<EnemyBase> TargetDefeated;
        public static event Action<EnemyBase> NormalDefeated;
        
        public static void OnEnemyDefeated(EnemyBase defeated) => EnemyDefeated?.Invoke(defeated);
        public static void OnTargetDefeated(EnemyBase target) => TargetDefeated?.Invoke(target);
        public static void OnNormalDefeated(EnemyBase normal) => NormalDefeated?.Invoke(normal);
    }
}