using Core.Behaviour.FiniteStateMachine;
using Core.Behaviour.FiniteStateMachine.StateImplementations;
using Player.States;

namespace Player {
    /// <summary>
    /// Record class to stone player states (aka enum)
    /// </summary>
    public class PlayerStates {
        public PlayerState IdleState { get; }
        public PlayerState WalkState { get; }
        public PlayerState JumpState { get; }
        public PlayerState AirborneState { get; }
        public PlayerState DashState { get; }

        public PlayerStates(StateMachine stateMachine, PlayerController player) {
            IdleState = new IdleState(stateMachine, player);
            WalkState = new WalkState(stateMachine, player);
            JumpState = new JumpState(stateMachine, player);
            AirborneState = new AirborneState(stateMachine, player);
            DashState = new DashState(stateMachine, player);
        }
    }
}