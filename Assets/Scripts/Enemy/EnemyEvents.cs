using System;
using UnityEngine;

namespace Enemy {
    public struct EnemyDefeatedInfo {
        public Type EnemyType { get; }
        public Vector3 Position { get; }
        public bool WasTarget { get; }
        public int ID { get; }
            
        public EnemyDefeatedInfo(Type type, int id, Vector3 position, bool wasTarget = false) {
            EnemyType = type;
            ID = id;
            Position = position;
            WasTarget = wasTarget;
        }
    }
    
    public static class EnemyEvents {
        public static event Action<EnemyDefeatedInfo> EnemyDefeated;
        public static event Action<EnemyDefeatedInfo> TargetDefeated;
        public static event Action<EnemyDefeatedInfo> NormalDefeated;
        
        public static void OnEnemyDefeated(EnemyDefeatedInfo defeated) => EnemyDefeated?.Invoke(defeated);
        public static void OnTargetDefeated(EnemyDefeatedInfo target) => TargetDefeated?.Invoke(target);
        public static void OnNormalDefeated(EnemyDefeatedInfo normal) => NormalDefeated?.Invoke(normal);
    }
}