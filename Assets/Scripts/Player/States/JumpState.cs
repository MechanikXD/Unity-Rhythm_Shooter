using Core.StateMachine.Base;
using Core.StateMachine.StateImplementations;
using UnityEngine;

namespace Player.States {
    public class JumpState : PlayerState {
        private readonly float _jumpUpwardsSpeed;
        private readonly Vector2 _jumpDurationBounds;
        private readonly float _airborneMoveSpeed;
        private readonly float _forwardJumpSpeedMultiplier;
        private float _currentJumpDuration;

        public JumpState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) {
            _jumpUpwardsSpeed = Player.VerticalJumpSpeed;
            _jumpDurationBounds = Player.JumpDurationBounds;
            _airborneMoveSpeed = Player.AirborneMoveSpeed;
            _forwardJumpSpeedMultiplier = Player.ForwardJumpSpeedMultiplier;
        }

        public override void EnterState() {
            PlayerEvents.OnJumpedEvent();
            _currentJumpDuration = 0f;
        }

        public override void FrameUpdate() {
            if (_currentJumpDuration < _jumpDurationBounds.x || 
                (Player.IsJumping && _currentJumpDuration < _jumpDurationBounds.y))  {
                
                _currentJumpDuration += Time.deltaTime;
                
                if (Player.DashKey.IsPressed() && !Player.DashInCooldown)
                    AttachedStateMachine.ChangeState(Player.States.DashState);

                Vector3 playerRelativeVector = Player.GetCameraRelativeVector(_airborneMoveSpeed);
                playerRelativeVector.y = _jumpUpwardsSpeed;
                playerRelativeVector += Player.CameraForward.normalized * _forwardJumpSpeedMultiplier;
                Player.Controller.Move(playerRelativeVector * Time.deltaTime);
            }
            else {
                Player.CurrentAirborneTime += _currentJumpDuration;
                AttachedStateMachine.ChangeState(Player.States.AirborneState);
            }
        }

        public override void ExitState() {
            Player.JumpKey.Reset();
        }
    }
}