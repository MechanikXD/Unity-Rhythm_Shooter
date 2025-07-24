using System;

namespace Core.Level.Room {
    public static class RoomEvents {
        public static event Action RoomEntered;
        public static event Action EnemyDefeated;
        public static event Action TargetDefeated;
        public static event Action CombatFinished;

        public static void OnRoomEntered() => RoomEntered?.Invoke();
        public static void OnEnemyDefeated() => EnemyDefeated?.Invoke();
        public static void OnTargetDefeated() => TargetDefeated?.Invoke();
        public static void OnCombatFinished() => CombatFinished?.Invoke();
    }
}