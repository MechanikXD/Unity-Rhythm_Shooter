using Core.StateMachine.Base;
using Core.StateMachine.StateImplementations;
using UnityEngine;

namespace Player.States {
    public class WalkState : PlayerState {
        private readonly float _moveSpeed;

        public WalkState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) {
            _moveSpeed = Player.MoveSpeed;
        }

        public override void EnterState() => PlayerEvents.OnStartWalkingEvent();

        public override void FrameUpdate() {
            if (!Player.IsGrounded) AttachedStateMachine.ChangeState(Player.States.AirborneState);
            
            if (!Player.IsMoving) AttachedStateMachine.ChangeState(Player.States.IdleState);
            
            if (Player.JumpKey.IsPressed()) AttachedStateMachine.ChangeState(Player.States.JumpState);
            
            if (Player.DashKey.IsPressed() && !Player.DashInCooldown)
                AttachedStateMachine.ChangeState(Player.States.DashState);
            
            var moveVector = Player.GetCameraRelativeVector(_moveSpeed);
            Player.Controller.Move(moveVector * Time.deltaTime);
        }

        public override void ExitState() => PlayerEvents.OnStoppedWalkingEvent();
    }
}