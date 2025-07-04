using System;

namespace Player {
    public static class PlayerEvents {
        public static event Action PlayerBecomeGroundedEvent;

        public static void OnPlayerBecomeGrounded() => PlayerBecomeGroundedEvent?.Invoke();
    }
}