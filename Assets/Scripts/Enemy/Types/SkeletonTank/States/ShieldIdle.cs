using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;

namespace Enemy.Types.SkeletonTank.States {
    public class ShieldIdle : EnemyState {
        public ShieldIdle(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates) 
            : base(stateMachine, enemy, outStates) { }
    }
}