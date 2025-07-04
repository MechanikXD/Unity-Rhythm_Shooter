using Core.StateMachine.Base;
using Core.StateMachine.StateImplementations;
using UnityEngine;

namespace Player.PlayerStates {
    public class JumpState : PlayerState {
        private readonly float _jumpUpwardsSpeed;
        private readonly float _jumpMaxDuration;
        private readonly float _airborneMoveSpeed;
        private readonly float _forwardJumpSpeedMultiplier;
        private float _currentJumpDuration;

        public JumpState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) {
            _jumpUpwardsSpeed = Player.VerticalJumpSpeed;
            _jumpMaxDuration = Player.JumpMaxDuration;
            _airborneMoveSpeed = Player.AirborneMoveSpeed;
            _forwardJumpSpeedMultiplier = Player.ForwardJumpSpeedMultiplier;
        }

        public override void EnterState() => _currentJumpDuration = 0f;

        public override void FrameUpdate() {
            if (Player.IsJumping && _currentJumpDuration < _jumpMaxDuration) {
                _currentJumpDuration += Time.deltaTime;
                
                if (Player.DashKey.IsPressed() && !Player.dashInCooldown)
                    AttachedStateMachine.ChangeState(Player.States.DashState);

                Vector3 playerRelativeVector = Player.GetCameraRelativeVector(_airborneMoveSpeed);
                playerRelativeVector.y = _jumpUpwardsSpeed;
                playerRelativeVector += Player.CameraForward.normalized * _forwardJumpSpeedMultiplier;
                Player.Controller.Move(playerRelativeVector * Time.deltaTime);
            }
            else {
                AttachedStateMachine.ChangeState(Player.States.AirborneState);
            }
        }

        public override void ExitState() {
            Player.JumpKey.Reset();
        }
    }
}