using Core.StateMachine.Base;
using Player;

namespace Core.StateMachine.StateImplementations {
    public class PlayerState : State {
        protected readonly PlayerController Player;
        protected PlayerState(Base.StateMachine stateMachine, PlayerController controller) :
            base(stateMachine) => Player = controller;
    }
}