using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;

namespace Enemy.Types.SkeletonTank.States {
    public class ShieldWalk : EnemyState {
        public ShieldWalk(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates) 
            : base(stateMachine, enemy, outStates) { }
    }
}