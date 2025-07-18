using Core.StateMachine.Base;
using Core.StateMachine.StateImplementations;

namespace Player.States {
    public class IdleState : PlayerState {
        public IdleState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) { }
        
        public override void FrameUpdate() {
            if (!Player.IsGrounded) AttachedStateMachine.ChangeState(Player.States.AirborneState);
            
            if (Player.IsMoving) AttachedStateMachine.ChangeState(Player.States.WalkState);
            
            if (Player.JumpKey.IsPressed()) AttachedStateMachine.ChangeState(Player.States.JumpState);
            
            if (Player.DashKey.IsPressed() && !Player.DashInCooldown)
                AttachedStateMachine.ChangeState(Player.States.DashState);
        }
    }
}