using System;

namespace Player {
    public static class PlayerEvents {
        public static event Action BecomeGroundedEvent;
        public static event Action JumpedEvent;
        public static event Action DashedEvent;
        public static event Action ExitedDashEvent;
        public static event Action StartWalkingEvent;
        public static event Action StoppedWalkingEvent;
        
        public static event Action LeftActionEvent;
        public static event Action RightActionEvent;
        public static event Action BothActionsEvent;

        public static void OnBecomeGrounded() => BecomeGroundedEvent?.Invoke();
        public static void OnStoppedWalkingEvent() => StoppedWalkingEvent?.Invoke();
        public static void OnStartWalkingEvent() => StartWalkingEvent?.Invoke();
        public static void OnExitedDashEvent() => ExitedDashEvent?.Invoke();
        public static void OnJumpedEvent() => JumpedEvent?.Invoke();
        public static void OnDashedEvent() => DashedEvent?.Invoke();

        public static void OnLeftAction() => LeftActionEvent?.Invoke();
        public static void OnRightAction() => RightActionEvent?.Invoke();
        public static void OnBothActions() => BothActionsEvent?.Invoke();
    }
}