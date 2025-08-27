using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;

namespace Enemy.States.Base {
    public abstract class EnemyState : State {
        protected readonly EnemyBase Enemy;
        protected EnemyState[] OutStates;

        protected EnemyState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates) : base(stateMachine) {
            Enemy = enemy;
            OutStates = outStates;
        }

        public void SetOutStates(EnemyState[] outStates) => OutStates = outStates;
    }
}