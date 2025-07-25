using System;

namespace Core.Level.Room {
    public static class RoomEvents {
        public static event Action RoomEntered;
        public static event Action CombatFinished;

        public static void OnRoomEntered() => RoomEntered?.Invoke();
        public static void OnCombatFinished() => CombatFinished?.Invoke();
    }
}