using Core.Behaviour.FiniteStateMachine;
using Player;

namespace Core.Behaviour.StateImplementations {
    public class PlayerState : State {
        protected readonly PlayerController Player;
        protected PlayerState(StateMachine stateMachine, PlayerController controller) :
            base(stateMachine) => Player = controller;
    }
}