using Core.StateMachine.Base;
using Core.StateMachine.StateImplementations;
using UnityEngine;

namespace Player.PlayerStates {
    public class AirborneState : PlayerState {
        private readonly float _airborneMoveSpeed;
        private readonly float _gravityMultiplier;
        
        public AirborneState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) {
            _airborneMoveSpeed = Player.AirborneMoveSpeed;
            _gravityMultiplier = Player.gravityMultiplier;
        }

        public override void FrameUpdate() {
            if (Player.IsGrounded) AttachedStateMachine.ChangeState(Player.States.IdleState);
            
            if (Player.DashKey.IsPressed() && !Player.dashInCooldown)
                AttachedStateMachine.ChangeState(Player.States.DashState);
            
            var moveVector = Player.GetCameraRelativeVector(_airborneMoveSpeed);
            var gravityVector = Physics.gravity * _gravityMultiplier;
            Player.Controller.Move((moveVector + gravityVector) * Time.deltaTime);
        }

        public override void ExitState() => PlayerEvents.OnPlayerBecomeGrounded();
    }
}