using Core.Behaviour.FiniteStateMachine;
using Core.Behaviour.FiniteStateMachine.StateImplementations;
using UnityEngine;

namespace Player.States {
    public class JumpState : PlayerState {
        private readonly float _jumpUpwardsSpeed;
        private readonly Vector2 _jumpDurationBounds;
        private readonly float _airborneMoveSpeed;
        private float _currentJumpDuration;
        private Vector3 _currentMoveVector;
        private const float LerpSpeed = 10f;

        private Vector3 _currentVelocity;
        private Vector3 _targetVelocity;

        public JumpState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) {
            _jumpUpwardsSpeed = Player.VerticalJumpSpeed;
            _jumpDurationBounds = Player.JumpDurationBounds;
            _airborneMoveSpeed = Player.AirborneMoveSpeed;
        }

        public override void EnterState() {
            PlayerEvents.OnJumpedEvent();
            _currentJumpDuration = 0f;
            
            _currentVelocity = Vector3.zero;
            _targetVelocity = Vector3.zero;
        }

        public override void FrameUpdate() {
            if (_currentJumpDuration < _jumpDurationBounds.x || 
                (Player.IsJumping && _currentJumpDuration < _jumpDurationBounds.y))  {
                
                _currentJumpDuration += Time.deltaTime;
                
                if (Player.DashKey.IsPressed() && !Player.DashInCooldown)
                    AttachedStateMachine.ChangeState(Player.States.DashState);
                
                Vector3 playerRelativeVector = Player.GetCameraRelativeVector(_airborneMoveSpeed);
                playerRelativeVector.y = _jumpUpwardsSpeed;
                _targetVelocity = playerRelativeVector;
    
                _currentVelocity =  Vector3.Lerp(_targetVelocity, _currentVelocity, LerpSpeed * Time.deltaTime);
    
                Player.Controller.Move(_currentVelocity * Time.deltaTime);
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