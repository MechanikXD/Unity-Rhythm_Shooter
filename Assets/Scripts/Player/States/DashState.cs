using Core.Behaviour.FiniteStateMachine;
using Core.Behaviour.StateImplementations;
using Core.Music;
using UnityEngine;

namespace Player.States {
    public class DashState : PlayerState {
        private Vector3 _dashVector;
        private float _currentDuration;
        
        private float _durationModifier;
        private float _speedModifier;

        public DashState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) { }
        
        public override void EnterState() {
            _currentDuration = 0f;
            OnPlayerDashed(Conductor.Instance.SongPosition);
            _dashVector = Player.GetCameraRelativeVector(Player.DashSpeed + _speedModifier);

            if (_dashVector != Vector3.zero) return;  
            // If standing still -> dash forward
            _dashVector = Player.CameraForward * Player.DashSpeed;
            _dashVector.y = 0;
            
            PlayerEvents.OnDashedEvent();
        }

        public override void FrameUpdate() {
            if (_currentDuration <= Player.DashDuration + _durationModifier) {
                _currentDuration += Time.deltaTime;
                Player.Controller.Move(_dashVector * Time.deltaTime);
            }
            else {
                Player.CurrentAirborneTime += _currentDuration;
                AttachedStateMachine.ChangeState(Player.States.AirborneState);
            }
        }

        public override void ExitState() {
            Player.StartDashCooldown();
            PlayerEvents.OnExitedDashEvent(); 
        }
        
        private void OnPlayerDashed(float songPosition) {
            var beatType = Conductor.Instance.GetBeatHitInfo(songPosition, ignoreDisabled:true);
            
            if (beatType.HitType == BeatHitType.Disabled) return;
            Conductor.Instance.SetInteractedThisBeat();
            PlayerActionEvents.OnPlayerDashed(beatType.HitType);

            switch (beatType.HitType) {
                case BeatHitType.Perfect:
                    // Faster dash: 125% faster and 75% of original duration
                    _speedModifier = Player.DashSpeed * 0.32f;
                    _durationModifier = -Player.DashDuration * 0.25f;
                    break;
                case BeatHitType.Good:
                    // Default params
                    _speedModifier = 0;
                    _durationModifier = 0;
                    break;
                case BeatHitType.Miss:
                    // Slower: 75 of original speed and 125% of duration
                    _speedModifier = -Player.DashSpeed * 0.32f;
                    _durationModifier = Player.DashDuration * 0.25f;
                    break;
            }
        }
    }
}