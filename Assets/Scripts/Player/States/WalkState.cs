using Core.Behaviour.FiniteStateMachine;
using Core.Behaviour.FiniteStateMachine.StateImplementations;
using Core.Music;
using UnityEngine;

namespace Player.States {
    public class WalkState : PlayerState {
        public WalkState(StateMachine stateMachine, PlayerController controller) : base(
            stateMachine, controller) { }

        public override void EnterState() {
            Conductor.NextBeat += PlayWalkSound;
            PlayerEvents.OnStartWalkingEvent();
        }

        private void PlayWalkSound() => Player.PlayRandomSound(Player.WalkSounds);

        public override void FrameUpdate() {
            if (!Player.IsGrounded) AttachedStateMachine.ChangeState(Player.States.AirborneState);
            
            if (!Player.IsMoving) AttachedStateMachine.ChangeState(Player.States.IdleState);
            
            if (Player.JumpKey.IsPressed()) AttachedStateMachine.ChangeState(Player.States.JumpState);
            
            if (Player.DashKey.IsPressed() && !Player.DashInCooldown)
                AttachedStateMachine.ChangeState(Player.States.DashState);
            
            var moveVector = Player.GetCameraRelativeVector(Player.CurrentSpeed);
            var gravityVector = Physics.gravity * Player._gravityMultiplier;
            Player.Controller.Move((moveVector + gravityVector) * Time.deltaTime);
        }

        public override void ExitState() {
            PlayerEvents.OnStoppedWalkingEvent();
            Conductor.NextBeat -= PlayWalkSound;
        }
    }
}