using Core.Behaviour.FiniteStateMachine;
using Core.Behaviour.FiniteStateMachine.StateImplementations;
using UnityEngine;

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
            
            var gravityVector = Physics.gravity * Player._gravityMultiplier;
            Player.Controller.Move(gravityVector * Time.deltaTime);
        }
    }
}