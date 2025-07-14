using Core.StateMachine.Base;
using Core.StateMachine.StateImplementations;
using UnityEngine;

namespace Player.PlayerStates {
    public class AirborneState : PlayerState {
        private readonly float _airborneMoveSpeed;
        private readonly float _gravityMultiplier;
        private readonly float _coyoteTime;
        
        public AirborneState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) {
            _airborneMoveSpeed = Player.AirborneMoveSpeed;
            _gravityMultiplier = Player.gravityMultiplier;
            _coyoteTime = Player.CoyoteTime;
        }

        public override void FrameUpdate() {
            Player.CurrentAirborneTime += Time.deltaTime;

            if (Player.IsGrounded) {
                AttachedStateMachine.ChangeState(Player.States.IdleState);
                PlayerEvents.OnBecomeGrounded();
                Player.CurrentAirborneTime = 0f;
            }
            
            if (Player.DashKey.IsPressed() && !Player.DashInCooldown)
                AttachedStateMachine.ChangeState(Player.States.DashState);
            
            if (Player.JumpKey.IsPressed() && Player.CurrentAirborneTime < _coyoteTime)
                AttachedStateMachine.ChangeState(Player.States.JumpState);
            
            var moveVector = Player.GetCameraRelativeVector(_airborneMoveSpeed);
            var gravityVector = Physics.gravity * _gravityMultiplier;
            Player.Controller.Move((moveVector + gravityVector) * Time.deltaTime);
        }
    }
}