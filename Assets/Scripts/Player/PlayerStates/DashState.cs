using Core.StateMachine.Base;
using Core.StateMachine.StateImplementations;
using UnityEngine;

namespace Player.PlayerStates {
    public class DashState : PlayerState {
        private Vector3 _dashVector;
        private readonly float _dashDuration;
        private float _currentDashDuration;

        public DashState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) {
            _dashDuration = Player.DashDuration;
        }
        
        public override void EnterState() {
            _currentDashDuration = 0f;
            _dashVector = Player.GetCameraRelativeVector(Player.DashSpeed);

            if (_dashVector != Vector3.zero) return;  
            // If standing still -> dash forward
            _dashVector = Player.CameraForward * Player.DashSpeed;
            _dashVector.y = 0;
            
            PlayerEvents.OnPlayerDashedEvent();
        }

        public override void FrameUpdate() {
            if (_currentDashDuration <= _dashDuration) {
                _currentDashDuration += Time.deltaTime;
                Player.Controller.Move(_dashVector * Time.deltaTime);
            }
            else {
                Player.CurrentAirborneTime += _currentDashDuration;
                AttachedStateMachine.ChangeState(Player.States.AirborneState);
            }
        }

        public override void ExitState() {
            Player.StartDashCooldown();
            PlayerEvents.OnPlayerExitedDashEvent();
        }
    }
}