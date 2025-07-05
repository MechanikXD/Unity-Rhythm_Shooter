using System;

namespace Player {
    public static class PlayerEvents {
        public static event Action PlayerBecomeGroundedEvent;
        public static event Action PlayerJumpedEvent;
        public static event Action PlayerDashedEvent;
        public static event Action PlayerExitedDashEvent;
        public static event Action PlayerStartWalkingEvent;
        public static event Action PlayerStoppedWalkingEvent;

        public static void OnPlayerBecomeGrounded() => PlayerBecomeGroundedEvent?.Invoke();
        public static void OnPlayerStoppedWalkingEvent() => PlayerStoppedWalkingEvent?.Invoke();
        public static void OnPlayerStartWalkingEvent() => PlayerStartWalkingEvent?.Invoke();
        public static void OnPlayerExitedDashEvent() => PlayerExitedDashEvent?.Invoke();
        public static void OnPlayerJumpedEvent() => PlayerJumpedEvent?.Invoke();
        public static void OnPlayerDashedEvent() => PlayerDashedEvent?.Invoke();
    }
}