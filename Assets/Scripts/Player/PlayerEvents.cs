using System;

namespace Player {
    public static class PlayerEvents {
        public static event Action BecomeGrounded;
        public static event Action Jumped;
        public static event Action Dashed;
        public static event Action ExitedDash;
        public static event Action StartWalking;
        public static event Action StoppedWalking;

        public static void OnBecomeGrounded() => BecomeGrounded?.Invoke();
        public static void OnStoppedWalkingEvent() => StoppedWalking?.Invoke();
        public static void OnStartWalkingEvent() => StartWalking?.Invoke();
        public static void OnExitedDashEvent() => ExitedDash?.Invoke();
        public static void OnJumpedEvent() => Jumped?.Invoke();
        public static void OnDashedEvent() => Dashed?.Invoke();
    }
}